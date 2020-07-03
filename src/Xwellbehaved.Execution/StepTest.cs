using System.Diagnostics.CodeAnalysis;

namespace Xwellbehaved.Execution
{

#if DEBUG
    using Validation;
#endif

    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved.Sdk;

    /// <inheritdoc cref="IStep"/>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Step", Justification = "By design.")]
    public class StepTest : LongLivedMarshalByRefObject, IStep
    {
        public StepTest(IScenario scenario, string displayName)
        {
            //Guard.AgainstNullArgument(nameof(scenario), scenario);

#if DEBUG
            scenario.RequiresNotNull(nameof(scenario));
#endif

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
