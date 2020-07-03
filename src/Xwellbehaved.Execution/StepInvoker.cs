using System;
using System.Threading;
using System.Threading.Tasks;

namespace Xwellbehaved.Execution
{

#if DEBUG
    using Validation;
#endif

    using Xunit.Sdk;
    using Xwellbehaved.Sdk;

    // TODO: TBD: can and probably should comment with Xml comments..
    public class StepInvoker
    {
        private readonly IStepContext _stepContext;
        private readonly Func<IStepContext, Task> _body;
        private readonly ExceptionAggregator _aggregator;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ExecutionTimer _timer = new ExecutionTimer();

        public StepInvoker(
            IStepContext stepContext
            , Func<IStepContext, Task> body
            , ExceptionAggregator aggregator
            , CancellationTokenSource cancellationTokenSource)
        {
            //Guard.AgainstNullArgument(nameof(aggregator), aggregator);
            //Guard.AgainstNullArgument(nameof(cancellationTokenSource), cancellationTokenSource);

#if DEBUG
            aggregator.RequiresNotNull(nameof(aggregator));
            cancellationTokenSource.RequiresNotNull(nameof(cancellationTokenSource));
#endif

            // TODO: TBD: #3 MWP 2020-07-01 03:15:09 PM / should we validate the other bits?
            this._stepContext = stepContext;
            this._body = body;
            this._aggregator = aggregator;
            this._cancellationTokenSource = cancellationTokenSource;
        }

        public async Task<decimal> RunAsync()
        {
            if (this._body != null)
            {
                await this._aggregator.RunAsync(async () =>
                {
                    if (!this._cancellationTokenSource.IsCancellationRequested && !this._aggregator.HasExceptions)
                    {
                        await Invoker.Invoke(() => this._body(this._stepContext), this._aggregator, this._timer);
                    }
                });
            }

            return this._timer.Total;
        }
    }
}
