using System;
using System.Linq;

namespace Xwellbehaved
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved.Infrastructure;

    // In order to have terse code
    // As a developer
    // I want to declare hold local state using scenario method parameters
    public class DefaultParametersFeature : Feature
    {
        private static class ScenarioWithFourParametersAndAStepAssertingEachIsADefaultValue
        {
            [Scenario]
            public static void Scenario(string w, int x, object y, int? z)
            {
                "Then w should be the default value of string".x(() => w.AssertEqual(default));

                "And x should be the default value of int".x(() => x.AssertEqual(default));

                "And y should be the default value of object".x(() => y.AssertEqual(default));

                "And z should be the default value of int?".x(() => z.AssertEqual(default));
            }
        }

        [Scenario]
        public void ScenarioWithParameters(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with four parameters and step asserting each is a default value".x(
                () => feature = typeof(ScenarioWithFourParametersAndAStepAssertingEachIsADefaultValue));

            "When I run the scenarios".x(() => results = this.Run<ITestResultMessage>(feature));

            "Then each result should be a pass".x(() => results.All(result => result is ITestPassed).AssertTrue());

            "And there should be 4 results".x(() => results.Length.AssertEqual(4));

            "And the display name of each result should not contain the parameter values".x(() =>
            {
                foreach (var result in results)
                {
                    result.Test.DisplayName.AssertDoesNotContain("w:");
                    result.Test.DisplayName.AssertDoesNotContain("x:");
                    result.Test.DisplayName.AssertDoesNotContain("y:");
                    result.Test.DisplayName.AssertDoesNotContain("z:");
                }
            });
        }
    }
}
