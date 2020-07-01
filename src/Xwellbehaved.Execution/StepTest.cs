using System.Diagnostics.CodeAnalysis;

namespace Xwellbehaved.Execution
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved.Sdk;

    /// <inheritdoc cref="IStep"/>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Step", Justification = "By design.")]
    public class StepTest : LongLivedMarshalByRefObject, IStep
    {
        public StepTest(IScenario scenario, string displayName)
        {
            // TODO: TBD: ditto fluently guard...
            Guard.AgainstNullArgument(nameof(scenario), scenario);

            this.Scenario = scenario;
            this.DisplayName = displayName;
        }

        /// <inheritdoc/>
        public IScenario Scenario { get; }

        /// <inheritdoc/>
        public string DisplayName { get; }

        /// <inheritdoc/>
        public ITestCase TestCase => this.Scenario.ScenarioOutline;
    }
}
