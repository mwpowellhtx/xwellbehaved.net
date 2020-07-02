using System;
using System.Linq;

namespace Xwellbehaved
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved.Infrastructure;

    public class ClassFixtureFeature : Feature
    {
        private class ScenarioWithAClassFixture : IClassFixture<Fixture>
        {
            private readonly Fixture _fixture;

            public ScenarioWithAClassFixture(Fixture fixture)
            {

#pragma warning disable IDE0021 // Use expression body for constructors
                this._fixture = fixture.AssertNotNull();
#pragma warning restore IDE0021 // Use expression body for constructors

            }

            [Scenario]
            public void Scenario1() => "Given".x(() => this._fixture.Scenario1Executed = true);

            [Scenario]
            public void Scenario2() => "Given".x(() => this._fixture.Scenario2Executed = true);
        }

        private sealed class Fixture : IDisposable
        {
            public bool Scenario1Executed { private get; set; }

            public bool Scenario2Executed { private get; set; }

            public void Dispose()
            {
                this.Scenario1Executed.AssertTrue();
                this.Scenario2Executed.AssertTrue();
                typeof(ClassFixtureFeature).SaveTestEvent("disposed");
            }
        }

        [Background]
        public void Background() => "Given no events have occurred".x(
            () => typeof(ClassFixtureFeature).EnqueueFeatureForDisposal(this));

        [Scenario]
        public void ClassFixture(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with a class fixture".x(
                () => feature = typeof(ScenarioWithAClassFixture));

            "When I run the scenario".x(() => results = this.Run<ITestResultMessage>(feature));

            "Then the class fixture is supplied as a constructor to each test class instance and disposed".x(() =>
            {
                results.All(result => result is ITestPassed).AssertTrue();
                typeof(ClassFixtureFeature).GetTestEvents().AssertEqual(new[] { "disposed" });
            });
        }
    }
}
