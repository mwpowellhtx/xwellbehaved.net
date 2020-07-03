using System;
using System.Linq;
using System.Reflection;

namespace Xwellbehaved
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved.Infrastructure;

    /// <summary>
    /// We think that the Feature verification is actually doing what was intended on being
    /// accomplished via Fixture disposal. However, we need to reconsider the approach in
    /// verifying this as the approacy may be a bit flawed. Because, if we understand the
    /// approach, the scenarios are going to visit one feature or the other, but never both
    /// together.
    /// </summary>
    public class CollectionFixtureFeature : Feature
    {
        public sealed class Fixture : IDisposable
        {
            private int _featureExecuted;

            private Type _visitedType;

            public void FeatureExecuted<T>(T host)
            {
                if (host is ScenarioWithACollectionFixture1)
                {
                    this._featureExecuted = 1;
                }
                else if (host is ScenarioWithACollectionFixture2)
                {
                    this._featureExecuted = 2;
                }

                if (this._featureExecuted > 0)
                {
                    this._visitedType = typeof(T);
                }
            }

            public void Dispose()
            {
                this._visitedType.AssertTrue(type =>
                    type == typeof(ScenarioWithACollectionFixture1)
                        || type == typeof(ScenarioWithACollectionFixture2));

                if (this._visitedType == typeof(ScenarioWithACollectionFixture1))
                {
                    this._featureExecuted.AssertEqual(1);
                }

                if (this._visitedType == typeof(ScenarioWithACollectionFixture2))
                {
                    this._featureExecuted.AssertEqual(2);
                }

                typeof(CollectionFixtureFeature).SaveTestEvent("disposed");
            }
        }

        [CollectionDefinition("CollectionFixtureTestFeatures")]
        public class CollectionFixtureTestFeatures : ICollectionFixture<Fixture>
        {
        }

        [Collection("CollectionFixtureTestFeatures")]
        public class ScenarioWithACollectionFixture1
        {
            private readonly Fixture _fixture;

            public ScenarioWithACollectionFixture1(Fixture fixture)
            {

#pragma warning disable IDE0021 // Use expression body for constructors
                this._fixture = fixture.AssertNotNull();
#pragma warning restore IDE0021 // Use expression body for constructors

            }

            [Scenario]
            public void Scenario1() => "Given".x(() => this._fixture.FeatureExecuted(this));
        }

        [Collection("CollectionFixtureTestFeatures")]
        public class ScenarioWithACollectionFixture2
        {
            private readonly Fixture _fixture;

            public ScenarioWithACollectionFixture2(Fixture fixture)
            {

#pragma warning disable IDE0021 // Use expression body for constructors
                this._fixture = fixture.AssertNotNull();
#pragma warning restore IDE0021 // Use expression body for constructors

            }

            [Scenario]
            public void Scenario1() => "Given".x(() => this._fixture.FeatureExecuted(this));
        }

        [Background]
        public void Background() => "Given no events have occurred".x(
            () => typeof(CollectionFixtureFeature).EnqueueFeatureForDisposal(this));

        [Scenario]
        public void CollectionFixture(string collectionName, ITestResultMessage[] results)
        {
            "Given features with a collection fixture".x(
                () => collectionName = "CollectionFixtureTestFeatures");

            "When I run the features".x(() => results = this.Run<ITestResultMessage>(
                typeof(CollectionFixtureFeature).GetTypeInfo().Assembly, collectionName));

            "Then the collection fixture is supplied as a constructor to each test class instance and disposed".x(() =>
            {
                results.All(result => result is ITestPassed).AssertTrue();
                typeof(CollectionFixtureFeature).GetTestEvents().AssertEqual(new[] { "disposed" });
            });
        }
    }
}
