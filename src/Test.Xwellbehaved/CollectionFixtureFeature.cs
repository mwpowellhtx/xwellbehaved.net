using System;
using System.Linq;
using System.Reflection;

namespace Xwellbehaved
{
    //using FluentAssertions;
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved.Infrastructure;

    public class CollectionFixtureFeature : Feature
    {
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
            public void Scenario1() => "Given".x(() => this._fixture.Feature1Executed());
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
            public void Scenario1() => "Given".x(() => this._fixture.Feature2Executed());
        }

        public sealed class Fixture : IDisposable
        {
            private bool _feature1Executed;
            private bool _feature2Executed;

            public void Feature1Executed() => this._feature1Executed = true;

            public void Feature2Executed() => this._feature2Executed = true;

            public void Dispose()
            {
                this._feature1Executed.AssertTrue();
                this._feature2Executed.AssertTrue();
                typeof(CollectionFixtureFeature).SaveTestEvent("disposed");
            }
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
