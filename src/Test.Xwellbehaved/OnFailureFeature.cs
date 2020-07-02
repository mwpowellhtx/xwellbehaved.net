using System;
using System.Linq;

namespace Xwellbehaved
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved.Infrastructure;

    public class OnFailureFeature : Feature
    {
        [Scenario]
        public void FailureBeforeForcedStep(Type feature, ITestResultMessage[] results)
        {
            ("Given a scenario with two empty steps, "
                + "a failing step with continuation, "
                + "two more empty steps, "
                + "a failing step and "
                + "another empty step").x(() => feature = typeof(Steps));

            "When I run the scenarios".x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be 7 results".x(() => results.Length.AssertEqual(7));

            "Then the first and second results are passes".x(
                () => results.Take(2).All(result => result is ITestPassed).AssertTrue());

            "And the third result is a failure".x(
                () => results.Skip(2).Take(1).All(result => result is ITestFailed).AssertTrue());

            "And the fourth and fifth results are passes".x(
                () => results.Skip(3).Take(2).All(result => result is ITestPassed).AssertTrue());

            "And the sixth result is a failure".x(
                () => results.Skip(5).Take(1).All(result => result is ITestFailed).AssertTrue());

            "And the seventh result is a skip".x(
                () => results.Skip(6).Take(1).All(result => result is ITestSkipped).AssertTrue());
        }

        private static class Steps
        {
            [Scenario]
            public static void Scenario()
            {
                "Given something".x(() => { });

                "When something".x(() => { });

                "Then something which fails".x(() => throw new InvalidOperationException("oops"))
                    .OnFailure(RemainingSteps.Run);

                "And something".x(() => { });

                "And something".x(() => { });

                "And something which fails".x(() => throw new InvalidOperationException("oops"));

                "And something".x(() => { });
            }
        }
    }
}
