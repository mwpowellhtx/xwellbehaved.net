using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Xwellbehaved.Execution
{
    using Xunit.Abstractions;
    using Xunit.Sdk;

    // TODO: TBD: ditto Xml comments...
    public class ScenarioOutlineTestCase : XunitTestCase
    {
        public ScenarioOutlineTestCase(
            IMessageSink diagnosticMessageSink
            , TestMethodDisplay defaultMethodDisplay
            , TestMethodDisplayOptions defaultMethodDisplayOpts
            , ITestMethod testMethod)
            : base(
                  diagnosticMessageSink
                  , defaultMethodDisplay
                  , defaultMethodDisplayOpts
                  , testMethod)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Never)
            , Obsolete("Called by the de-serializer", true)]
        public ScenarioOutlineTestCase()
        {
        }

        public override async Task<RunSummary> RunAsync(
            IMessageSink diagnosticMessageSink
            , IMessageBus messageBus
            , object[] constructorArguments
            , ExceptionAggregator aggregator
            , CancellationTokenSource cancellationTokenSource) =>
            await new ScenarioOutlineTestCaseRunner(
                diagnosticMessageSink
                , this
                , this.DisplayName
                , this.SkipReason
                , constructorArguments
                , messageBus
                , aggregator
                , cancellationTokenSource)
            .RunAsync();
    }
}
