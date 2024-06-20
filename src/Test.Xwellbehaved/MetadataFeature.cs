namespace Xwellbehaved
{
    using Sdk;
    using Xunit;

    public class MetadataFeature
    {
        /// <summary>
        /// Critical in this Scenario is <paramref name="text"/>, as are the other bits.
        /// <c>text</c> is unused throughout the body of the function, but is critical in
        /// order for the caller to make the proper connection upon invocation.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="stepContext"></param>
        /// <param name="step"></param>
        /// <param name="scenario"></param>
        [Scenario
            , Example("abc")]
#pragma warning disable IDE0060 // Remove unused parameter 'text' if it is not part of a shipped public API
        public void UsingMetadata(string text, IStepContext stepContext, IStep step, IScenario scenario)
        {
            "When I execute a step".x(context => stepContext = context)
                .Teardown(context => context.AssertSame(stepContext));

            "Then the step context contains metadata about the step".x(() =>
            {
                // TODO: ditto fluent inconsistencies
                stepContext.Step.AssertNotNull().AssertIsAssignableTo<IStep>();
                step = stepContext.Step;
                var stepDisplayName = step.DisplayName;
                stepDisplayName.AssertEqual($"Xwellbehaved.MetadataFeature.UsingMetadata(text: \"abc\") [01] ({StepType.Scenario}): When I execute a step");
            });

            "And the step contains metadata about the scenario".x(() =>
            {
                // TODO: ditto
                step.Scenario.AssertNotNull().AssertIsAssignableTo<IScenario>();
                scenario = step.Scenario;
                var scenarioDisplayName = scenario.DisplayName;
                scenarioDisplayName.AssertEqual("Xwellbehaved.MetadataFeature.UsingMetadata(text: \"abc\")");
            });

            "And the step contains metadata about the scenario outline".x(() =>
            {
                var scenarioScenarioOutline = scenario.ScenarioOutline;
                scenarioScenarioOutline.AssertNotNull();
                scenarioScenarioOutline.DisplayName.AssertEqual("Xwellbehaved.MetadataFeature.UsingMetadata");
            });
        }
    }
}
