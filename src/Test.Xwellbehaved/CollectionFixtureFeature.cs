using System;
using System.Reflection;

namespace Xwellbehaved
{
    using FluentAssertions;
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
            private readonly Fixture fixture;

            public ScenarioWithACollectionFixture1(Fixture fixture)
            {
                fixture.Should().NotBeNull();
                this.fixture = fixture;
            }

            [Scenario]
            public void Scenario1() => "Given".x(() => this.fixture.Feature1Executed());
        }

        [Collection("CollectionFixtureTestFeatures")]
        public class ScenarioWithACollectionFixture2
        {
            private readonly Fixture fixture;

            public ScenarioWithACollectionFixture2(Fixture fixture)
            {
                fixture.Should().NotBeNull();
                this.fixture = fixture;
            }

            [Scenario]
            public void Scenario1() => "Given".x(() => this.fixture.Feature2Executed());
        }

        public sealed class Fixture : IDisposable
        {
            private bool feature1Executed;
            private bool feature2Executed;

            public void Feature1Executed() => this.feature1Executed = true;

            public void Feature2Executed() => this.feature2Executed = true;

            public void Dispose()
            {
                this.feature1Executed.Should().BeTrue();
                this.feature2Executed.Should().BeTrue();
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

            "When I run the features".x(
                () => results = this.Run<ITestResultMessage>(
                    typeof(CollectionFixtureFeature).GetTypeInfo().Assembly, collectionName));

            "Then the collection fixture is supplied as a constructor to each test class instance and disposed".x(() =>
            {
                results.Should().ContainItemsAssignableTo<ITestPassed>();
                typeof(CollectionFixtureFeature).GetTestEvents().Should().Equal("disposed");
            });
        }
    }
}
