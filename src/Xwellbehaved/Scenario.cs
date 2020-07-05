namespace Xwellbehaved.Execution
{
    using Xunit;
    using Xunit.Abstractions;
    using Xunit.Sdk;
    using Xwellbehaved.Sdk;

    /// <inheritdoc cref="IScenario"/>
    public class Scenario : LongLivedMarshalByRefObject, IScenario
    {
        public Scenario(IXunitTestCase scenarioOutline, string displayName)
        {
            this.ScenarioOutline = scenarioOutline;
            this.DisplayName = displayName;
        }

        /// <inheritdoc/>
        public IXunitTestCase ScenarioOutline { get; }

        /// <inheritdoc/>
        public string DisplayName { get; }

        /// <inheritdoc/>
        public ITestCase TestCase => this.ScenarioOutline;
    }
}
