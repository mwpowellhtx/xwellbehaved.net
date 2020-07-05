using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Xwellbehaved.Execution
{
    using Xunit.Abstractions;
    using Xunit.Sdk;
    using Xwellbehaved.Sdk;

    /// <inheritdoc/>
    public class StepTestRunner : XunitTestRunner
    {
        private readonly IStepContext _stepContext;
        private readonly Func<IStepContext, Task> _body;

        public StepTestRunner(
            IStepContext stepContext
            , Func<IStepContext, Task> body
            , ITest test
            , IMessageBus messageBus
            , Type scenarioClass
            , object[] constructorArguments
            , MethodInfo scenarioMethod
            , object[] scenarioMethodArguments
            , string skipReason
            , IReadOnlyList<BeforeAfterTestAttribute> beforeAfterAttributes
            , ExceptionAggregator aggregator
            , CancellationTokenSource cancellationTokenSource)
            : base(
                  test
                  , messageBus
                  , scenarioClass
                  , constructorArguments
                  , scenarioMethod
                  , scenarioMethodArguments
                  , skipReason
                  , beforeAfterAttributes
                  , aggregator
                  , cancellationTokenSource)
        {
            this._stepContext = stepContext;
            this._body = body;
        }

        /// <inheritdoc/>
        protected override Task<decimal> InvokeTestMethodAsync(ExceptionAggregator aggregator) =>
            new StepInvoker(this._stepContext, this._body, aggregator, this.CancellationTokenSource).RunAsync();
    }
}
