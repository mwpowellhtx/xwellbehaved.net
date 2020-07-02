using System;
using System.Collections.Generic;
using System.Linq;

namespace Xwellbehaved
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved.Sdk;
    using Xwellbehaved.Infrastructure;

    public class StepDefinitionFilterFeature : Feature
    {
        private sealed class SkipAllAttribute : Attribute, IFilter<IStepDefinition>
        {
            public IEnumerable<IStepDefinition> Filter(IEnumerable<IStepDefinition> steps) =>
                steps.Select(step => step.Skip("test"));
        }

        private class ScenarioWithSkipAll
        {
            [Scenario
                , SkipAll]
            public void Scenario()
            {
                "Given something".x(() => { });

                "When something".x(() => { });

                "Then something".x(() => { });
            }
        }

        private sealed class ContinueAfterThenAttribute : Attribute, IFilter<IStepDefinition>
        {
            public IEnumerable<IStepDefinition> Filter(IEnumerable<IStepDefinition> steps)
            {
                var then = false;
                return steps.Select(step => step.OnFailure(
                    then || (then = step.Text.StartsWith("Then ", StringComparison.OrdinalIgnoreCase))
                    ? RemainingSteps.Run
                    : RemainingSteps.Skip));
            }
        }

        private class ScenarioWithContinueAfterThen
        {
            [Scenario
                , ContinueAfterThen]
            public void Scenario()
            {
                "Given something".x(() => { });

                "When something".x(() => { });

                "Then something".x(() => throw new InvalidOperationException());

                "And something".x(() => { });
            }
        }

        private sealed class BackgroundSuffixesAttribute : Attribute, IFilter<IStepDefinition>
        {
            public IEnumerable<IStepDefinition> Filter(IEnumerable<IStepDefinition> steps) =>
                steps.Select(step => step.DisplayText((stepText, isBackgroundStep) =>
                    stepText + (isBackgroundStep ? " (Background)" : null)));
        }

        private class ScenarioWithBackgroundSuffixes
        {
            [Background]
            public void Background() => "Given something".x(() => { });

            [Scenario]
            [BackgroundSuffixes]
            public void Scenario()
            {
                "Given something".x(() => { });

                "When something".x(() => { });

                "Then something".x(() => throw new InvalidOperationException());

                "And something".x(() => { });
            }
        }

        [Scenario]
        public void SkipAll(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario marked with SkipAll".x(
                () => feature = typeof(ScenarioWithSkipAll));

            "When I run the scenario".x(() => results = this.Run<ITestResultMessage>(feature));

            "And there are results".x(() => results.AssertNotEmpty());

            "Then the steps are skipped".x(() => results.All(result => result is ITestSkipped).AssertTrue());
        }

        [Scenario]
        public void ContinueAfterThen(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario marked with ContinueAfterThen".x(
                () => feature = typeof(ScenarioWithContinueAfterThen));

            "When I run the scenario".x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there are four results".x(() => results.Length.AssertEqual(4));

            "Then the first two steps pass".x(() => results.Take(2).All(result => result is ITestPassed).AssertTrue());

            "And the third step fails".x(() => results.Skip(2).Take(1).All(result => result is ITestFailed).AssertTrue());

            "And the fourth step passes".x(() => results.Skip(3).Take(1).All(result => result is ITestPassed).AssertTrue());
        }

        [Scenario]
        public void BackgroundSuffixes(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario marked with BackgroundSuffixes".x(
                () => feature = typeof(ScenarioWithBackgroundSuffixes));

            "When I run the scenario".x(
                () => results = this.Run<ITestResultMessage>(feature));

            "Then the first result has a background suffix".x(() => results[0].Test.DisplayName.AssertEndsWith("(Background)"));
        }
    }
}
