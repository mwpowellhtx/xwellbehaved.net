using System;

namespace Xwellbehaved
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved.Infrastructure;

    /// <summary>
    /// In order to commit largely incomplete features as a developer I want to temporarily skip
    /// an entire scenario.
    /// </summary>
    public class SkippedScenarioFeature : Feature
    {
        private static class FeatureWithASkippedScenario
        {

#pragma warning disable xUnit1004 // Test methods should not be skipped
            [Scenario(Skip = "Test")]
#pragma warning restore xUnit1004 // Test methods should not be skipped
            public static void Scenario1() => throw new InvalidOperationException();
        }

        [Scenario]
        public void SkippedScenario(Type feature, ITestResultMessage[] results)
        {
            "Given a feature with a skipped scenario".x(
                () => feature = typeof(FeatureWithASkippedScenario));

            "When I run the scenarios".x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one result".x(() => results.AssertEqual(1, x => x.Length));

            "And the result should be a skip result".x(() => results[0].AssertIsAssignableTo<ITestSkipped>());
        }
    }
}
