using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Xwellbehaved.Execution
{
    using Xunit.Abstractions;
    using Xunit.Sdk;
    using Xwellbehaved.Execution.Extensions;

    /// <inheritdoc/>
    public class ScenarioOutlineTestCaseRunner : XunitTestCaseRunner
    {
        private static readonly object[] _noArguments = Array.Empty<object>();

        private readonly IMessageSink _diagnosticMessageSink;
        private readonly ExceptionAggregator _cleanupAggregator = new ExceptionAggregator();
        private readonly List<ScenarioRunner> _scenarioRunners = new List<ScenarioRunner>();
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private Exception _dataDiscoveryException;

        public ScenarioOutlineTestCaseRunner(
            IMessageSink diagnosticMessageSink
            , IXunitTestCase scenarioOutline
            , string displayName
            , string skipReason
            , object[] constructorArguments
            , IMessageBus messageBus
            , ExceptionAggregator aggregator
            , CancellationTokenSource cancellationTokenSource)
            : base(
                  scenarioOutline
                  , displayName
                  , skipReason
                  , constructorArguments
                  , _noArguments
                  , messageBus
                  , aggregator
                  , cancellationTokenSource)
        {
            this._diagnosticMessageSink = diagnosticMessageSink;
        }

        /// <inheritdoc/>
        protected override async Task AfterTestCaseStartingAsync()
        {
            await base.AfterTestCaseStartingAsync();

            try
            {
                var dataAttributes = this.TestCase.TestMethod.Method.GetCustomAttributes(typeof(DataAttribute)).ToList();
                foreach (var dataAttribute in dataAttributes)
                {
                    var discovererAttribute = dataAttribute.GetCustomAttributes(typeof(DataDiscovererAttribute)).First();
                    var discoverer =
                        ExtensibilityPointFactory.GetDataDiscoverer(this._diagnosticMessageSink, discovererAttribute);

                    foreach (var dataRow in discoverer.GetData(dataAttribute, this.TestCase.TestMethod.Method))
                    {
                        this._disposables.AddRange(dataRow.OfType<IDisposable>());

                        var info = new ScenarioInfo(this.TestCase.TestMethod.Method, dataRow, this.DisplayName);
                        var methodToRun = info.MethodToRun;
                        var convertedDataRow = info.ConvertedDataRow.ToArray();

                        var theoryDisplayName = info.ScenarioDisplayName;
                        var test = new Scenario(this.TestCase, theoryDisplayName);
                        var skipReason = this.SkipReason ?? dataAttribute.GetNamedArgument<string>("Skip");
                        var runner = new ScenarioRunner(test, this.MessageBus, this.TestClass, this.ConstructorArguments, methodToRun, convertedDataRow, skipReason, this.BeforeAfterAttributes, this.Aggregator, this.CancellationTokenSource);
                        this._scenarioRunners.Add(runner);
                    }
                }

                if (!this._scenarioRunners.Any())
                {
                    var info = new ScenarioInfo(this.TestCase.TestMethod.Method, _noArguments, this.DisplayName);
                    var methodToRun = info.MethodToRun;
                    var convertedDataRow = info.ConvertedDataRow.ToArray();

                    var theoryDisplayName = info.ScenarioDisplayName;
                    var test = new Scenario(this.TestCase, theoryDisplayName);
                    var runner = new ScenarioRunner(test, this.MessageBus, this.TestClass, this.ConstructorArguments, methodToRun, convertedDataRow, this.SkipReason, this.BeforeAfterAttributes, this.Aggregator, this.CancellationTokenSource);
                    this._scenarioRunners.Add(runner);
                }
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                this._dataDiscoveryException = ex;
            }
        }

        /// <inheritdoc/>
        protected override async Task<RunSummary> RunTestAsync()
        {
            if (this._dataDiscoveryException != null)
            {
                this.MessageBus.Queue(new XunitTest(this.TestCase, this.DisplayName)
                    , test => new TestFailed(test, 0, null, this._dataDiscoveryException.Unwrap())
                    , this.CancellationTokenSource);

                return new RunSummary { Total = 1, Failed = 1 };
            }

            var summary = new RunSummary();
            foreach (var scenarioRunner in this._scenarioRunners)
            {
                summary.Aggregate(await scenarioRunner.RunAsync());
            }

            /* Run the cleanup here so we can include cleanup time in the run summary, but save
             * any exceptions so we can surface them during the cleanup phase, so they get
             * properly reported as test case cleanup failures. */

            var timer = new ExecutionTimer();
            foreach (var disposable in this._disposables)
            {
                timer.Aggregate(() => this._cleanupAggregator.Run(() => disposable.Dispose()));
            }

            summary.Time += timer.Total;
            return summary;
        }

        /// <inheritdoc/>
        protected override Task BeforeTestCaseFinishedAsync()
        {
            this.Aggregator.Aggregate(this._cleanupAggregator);

            return base.BeforeTestCaseFinishedAsync();
        }
    }
}
