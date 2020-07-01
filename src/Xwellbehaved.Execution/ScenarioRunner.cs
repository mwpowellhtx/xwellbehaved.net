using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Xwellbehaved.Execution
{
    using Xunit.Sdk;
    using Xwellbehaved.Execution.Extensions;
    using Xwellbehaved.Sdk;

    // TODO: TBD: can and probably should comment with Xml comments...
    public class ScenarioRunner
    {
        private readonly IScenario _scenario;
        private readonly IMessageBus _messageBus;
        private readonly Type _scenarioClass;
        private readonly object[] _constructorArguments;
        private readonly MethodInfo _scenarioMethod;
        private readonly object[] _scenarioMethodArguments;
        private readonly string _skipReason;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> _beforeAfterScenarioAttributes;
        private readonly ExceptionAggregator _parentAggregator;
        private readonly CancellationTokenSource _cancellationTokenSource;

        public ScenarioRunner(
            IScenario scenario
            , IMessageBus messageBus
            , Type scenarioClass
            , object[] constructorArguments
            , MethodInfo scenarioMethod
            , object[] scenarioMethodArguments
            , string skipReason
            , IReadOnlyList<BeforeAfterTestAttribute> beforeAfterScenarioAttributes
            , ExceptionAggregator aggregator
            , CancellationTokenSource cancellationTokenSource)
        {
            // TODO: TBD: ditto fluently guard.
            Guard.AgainstNullArgument(nameof(scenario), scenario);
            Guard.AgainstNullArgument(nameof(messageBus), messageBus);
            Guard.AgainstNullArgument(nameof(aggregator), aggregator);

            this._scenario = scenario;
            this._messageBus = messageBus;
            this._scenarioClass = scenarioClass;
            this._constructorArguments = constructorArguments;
            this._scenarioMethod = scenarioMethod;
            this._scenarioMethodArguments = scenarioMethodArguments;
            this._skipReason = skipReason;
            this._beforeAfterScenarioAttributes = beforeAfterScenarioAttributes;
            this._parentAggregator = aggregator;
            this._cancellationTokenSource = cancellationTokenSource;
        }

        public async Task<RunSummary> RunAsync()
        {
            if (!string.IsNullOrEmpty(this._skipReason))
            {
                this._messageBus.Queue(
                    this._scenario
                    , test => new TestSkipped(test, this._skipReason)
                    , this._cancellationTokenSource);

                return new RunSummary { Total = 1, Skipped = 1 };
            }
            else
            {
                var summary = new RunSummary();
                var childAggregator = new ExceptionAggregator(this._parentAggregator);
                if (!childAggregator.HasExceptions)
                {
                    summary.Aggregate(await childAggregator.RunAsync(() => this.InvokeScenarioAsync(childAggregator)));
                }

                var exception = childAggregator.ToException();
                if (exception != null)
                {
                    summary.Total++;
                    summary.Failed++;
                    this._messageBus.Queue(
                        this._scenario
                        , test => new TestFailed(test, summary.Time, string.Empty, exception)
                        , this._cancellationTokenSource);
                }
                else if (summary.Total == 0)
                {
                    summary.Total++;
                    this._messageBus.Queue(
                        this._scenario
                        , test => new TestPassed(test, summary.Time, string.Empty)
                        , this._cancellationTokenSource);
                }

                return summary;
            }
        }

        private async Task<RunSummary> InvokeScenarioAsync(ExceptionAggregator aggregator) =>
            await new ScenarioInvoker(
                this._scenario
                , this._messageBus
                , this._scenarioClass
                , this._constructorArguments
                , this._scenarioMethod
                , this._scenarioMethodArguments
                , this._beforeAfterScenarioAttributes
                , aggregator
                , this._cancellationTokenSource)
            .RunAsync();
    }
}
