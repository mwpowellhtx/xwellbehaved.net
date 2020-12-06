using System;
using System.Linq;

namespace Xwellbehaved
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved.Infrastructure;
    using Xwellbehaved.Sdk;

    /// <summary>
    /// In order to write less code as a developer I want to add background steps to all the
    /// scenarios in a feature.
    /// </summary>
    public class BackgroundFeature : Feature
    {
        private static class BackgroundWithTwoStepsAndTwoScenariosEachWithTwoSteps
        {
            private static int x;

            [Background]
            public static void Background()
            {
                "Given x is incremented".x(() => ++x);

                "And x is incremented again".x(() => ++x);
            }

            [Scenario]
            public static void Scenario1()
            {
                "Given x is 2".x(() => x.AssertEqual(2));

                "Then I set x to 0".x(() => x = 0);
            }

            [Scenario]
            public static void Scenario2()
            {
                "Given x is 2".x(() => x.AssertEqual(2));

                "Then I set x to 0".x(() => x = 0);
            }
        }

        private class Base
        {
            protected int X { get; set; }

            [Background]
            public void Background()
            {
                "Given x is incremented".x(() => ++this.X);

                "And x is incremented again".x(() => ++this.X);
            }
        }

        private class BackgroundInBaseTypeWithTwoStepsAndTwoScenariosEachWithTwoSteps : Base
        {
            [Scenario]
            public void Scenario1()
            {
                "Given x is 2".x(() => this.X.AssertEqual(2));

                "Then I set x to 0".x(() => this.X = 0);
            }

            [Scenario]
            public void Scenario2()
            {
                "Given x is 2".x(() => this.X.AssertEqual(2));

                "Then I set x to 0".x(() => this.X = 0);
            }
        }

        [Scenario
            , Example(typeof(BackgroundWithTwoStepsAndTwoScenariosEachWithTwoSteps))
            , Example(typeof(BackgroundInBaseTypeWithTwoStepsAndTwoScenariosEachWithTwoSteps))]
        public void BackgroundSteps(Type feature, ITestResultMessage[] results)
        {
            $"Given a {feature}".x(() => { });

            "When I run the scenarios".x(() => results = this.Run<ITestResultMessage>(feature));

            "Then the background steps are run before each scenario".x(() => results.All(x => x is ITestPassed).AssertTrue());

            "And there are eight results".x(() => results.Length.AssertEqual(8));

            $"And the background steps have '({StepType.Background})' in their names".x(() =>
            {
                foreach (var result in results.Take(2).Concat(results.Skip(4).Take(2)))
                {
                    result.Test.DisplayName.AssertContains($"({StepType.Background})");
                }
            });
        }
    }
}
