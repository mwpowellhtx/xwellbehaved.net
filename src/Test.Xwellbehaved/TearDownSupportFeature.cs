using System;
using System.Linq;

namespace Xwellbehaved
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved.Infrastructure;
    using Xwellbehaved.Sdk;

    /// <summary>
    /// In order to write less code as a developer I want to add TearDown steps to all the
    /// scenarios in a feature.
    /// </summary>
    public class TearDownSupportFeature : Feature
    {
        /// <summary>
        /// Minding the fact that these are actually static test fixtures.
        /// Yes, static is a thing.
        /// </summary>
        private static class TearDownWithTwoStepsAndTwoScenariosEachWithTwoSteps
        {
            private static int x;

            private const int zed = default;

            [TearDown]
            public static void TearDownOne(int y)
            {
                "Assume y was x".x(() => y = x);

                "Given x is decremented".x(() => --x);

                "And x should be y-1".x(() => x.AssertEqual(y - 1));
            }

            [Scenario]
            public static void Scenario1()
            {
                "Verify x at least zero".x(() => x.AssertTrue(y => y >= zed));

                "Then I set x to 1".x(() => x = 1);
            }

            [Scenario]
            public static void Scenario2()
            {
                "Verify x at least zero".x(() => x.AssertTrue(y => y >= zed));

                "Then I set x to 2".x(() => x = 2);
            }
        }

        private class Base
        {
            protected int X { get; set; }

            [TearDown]
            public void TearDown(int y)
            {
                "Assume y was X".x(() => y = this.X);

                "Given X is decremented".x(() => --this.X);

                "And X should be y-1".x(() => this.X.AssertEqual(y - 1));
            }
        }

        private class TearDownInBaseTypeWithTwoStepsAndTwoScenariosEachWithTwoSteps : Base
        {
            [Scenario]
            public void Scenario1()
            {
                "Given X is zero".x(() => this.X.AssertEqual(default));

                "Then I set X to 1".x(() => this.X = 1);
            }

            [Scenario]
            public void Scenario2()
            {
                "Given X is zero".x(() => this.X.AssertEqual(default));

                "Then I set X to 2".x(() => this.X = 2);
            }
        }

        [Scenario
            , Example(typeof(TearDownWithTwoStepsAndTwoScenariosEachWithTwoSteps))
            , Example(typeof(TearDownInBaseTypeWithTwoStepsAndTwoScenariosEachWithTwoSteps))]
        public void TearDownSteps(Type feature, ITestResultMessage[] results)
        {
            $"Given a {feature}".x(() => { });

            "When I run the scenarios".x(() => results = this.Run<ITestResultMessage>(feature));

            "Then the teardown steps are run before each scenario".x(() => results.All(x => x is ITestPassed).AssertTrue());

            "And there are eight results".x(() => results.Length.AssertEqual(10));

            $"And the teardown steps have '({StepType.TearDown})' in their names".x(() =>
            {
                foreach (var result in results.Skip(2).Take(3).Concat(results.Skip(7).Take(3)))
                {
                    result.Test.DisplayName.AssertContains($"({StepType.TearDown})");
                }
            });
        }
    }
}
