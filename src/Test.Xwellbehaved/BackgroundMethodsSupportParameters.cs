using System;
using System.Linq;

namespace Xwellbehaved
{
    using Infrastructure;
    using Xunit;
    using Xunit.Abstractions;

    // #4 MWP 2020-07-09 05:25:27 PM / Background methods support parameter default values.
    public class BackgroundMethodsSupportParameters : Feature
    {
        private class ParameterDefaultValuesAreSupported
        {
            [Background]
            public void BackgroundWithParameters(int @int, bool @bool, string @string, Guid guid, Guid? nullableGuid, Feature feature)
            {
                $"{nameof(@int)} is default".x(() => @int.AssertEqual(default));

                $"{nameof(@bool)} is default".x(() => @bool.AssertEqual(default));

                $"{nameof(@string)} is default".x(() => @string.AssertEqual(default));

                $"{nameof(guid)} is default".x(() => guid.AssertEqual(default));

                $"{nameof(nullableGuid)} is default".x(() => nullableGuid.AssertEqual(default));

                $"{nameof(feature)} is default".x(() => feature.AssertEqual(default));
            }

            [Scenario]
            public void The_fact_we_got_here_is_evidence_enough() => "true is true".x(() => true.AssertTrue());
        }

        private class TestsWithBackgroundWithoutParameters
        {
            [Background]
            public void BackgroundWithoutParameters() => "1+1 is still 2".x(() => (1 + 1).AssertEqual(2));

            [Scenario]
            public void The_fact_we_got_here_is_evidence_enough() => "true is true".x(() => true.AssertTrue());
        }

        /// <summary>
        /// We do not need 
        /// </summary>
        [Scenario]
        public void Backgrounds_can_support_parameters_default_values(Type feature, ITestResultMessage[] results)
        {
            "Given test with a background with parameters".x(
                () => feature = typeof(ParameterDefaultValuesAreSupported));

            "When I run the scenario".x(() => results = this.Run<ITestResultMessage>(feature));

            "Then expecting passing results".x(() =>
                results.All(result => result is ITestPassed).AssertTrue());
        }

        /// <summary>
        /// We do not need 
        /// </summary>
        [Scenario]
        public void Backgrounds_without_parameters_are_still_okay(Type feature, ITestResultMessage[] results)
        {
            "Given test with a background without parameters".x(
                () => feature = typeof(TestsWithBackgroundWithoutParameters));

            "When I run the scenario".x(() => results = this.Run<ITestResultMessage>(feature));

            "Then expecting passing results".x(() =>
                results.All(result => result is ITestPassed).AssertTrue());
        }
    }
}
