namespace Xwellbehaved.Sdk
{
    /// <summary>
    /// Gets the display text for a step.
    /// </summary>
    /// <param name="stepText">The step text.</param>
    /// <param name="stepDefinitionType">The Type of StepDefinition that is being reported.</param>
    /// <returns>A string representing the display text for the step.</returns>
    public delegate string GetStepDisplayText(string stepText, StepType stepDefinitionType);
}
