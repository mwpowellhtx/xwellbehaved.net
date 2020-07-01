namespace Xwellbehaved
{
    using FluentAssertions;
    using Xunit.Sdk;
    using Xwellbehaved.Sdk;

    public class MetadataFeature
    {
        [Scenario
            , Example("abc")]
        public void UsingMetadata(string text, IStepContext stepContext, IStep step, IScenario scenario)
        {
            "When I execute a step".x(c => stepContext = c)
                .Teardown(c => c.Should().BeSameAs(stepContext));

            "Then the step context contains metadata about the step".x(() =>
            {
                step = stepContext.Step.Should().NotBeNull().And.Subject.As<IStep>();
                var stepDisplayName = step.DisplayName;
                stepDisplayName.Should().Be("Xwellbehaved.MetadataFeature.UsingMetadata(text: \"abc\") [01] When I execute a step");
            });

            "And the step contains metadata about the scenario".x(() =>
            {
                scenario = step.Scenario.Should().NotBeNull().And.Subject.As<IScenario>();
                var scenarioDisplayName = scenario.DisplayName;
                scenarioDisplayName.Should().Be("Xwellbehaved.MetadataFeature.UsingMetadata(text: \"abc\")");
            });

            "And the step contains metadata about the scenario outline".x(() =>
            {
                var scenarioScenarioOutline = scenario.ScenarioOutline;
                scenarioScenarioOutline.Should().NotBeNull().And.Subject.As<IXunitTestCase>()
                    .DisplayName.Should().Be("Xwellbehaved.MetadataFeature.UsingMetadata");
            });
        }
    }
}
