using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Xwellbehaved.Execution
{
    using Xunit.Abstractions;
    using Xunit.Sdk;
    using Xwellbehaved.Execution.Extensions;
    using Xwellbehaved.Sdk;

    // TODO: TBD: we can and probably should comment these in Xml fashion...
    public class ScenarioInvoker
    {
        private readonly IScenario _scenario;
        private readonly IMessageBus _messageBus;
        private readonly Type _scenarioClass;
        private readonly object[] _constructorArguments;
        private readonly MethodInfo _scenarioMethod;
        private readonly object[] _scenarioMethodArguments;
        private readonly IReadOnlyList<BeforeAfterTestAttribute> _beforeAfterScenarioAttributes;
        private readonly ExceptionAggregator _aggregator;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly ExecutionTimer _timer = new ExecutionTimer();
        private readonly Stack<BeforeAfterTestAttribute> _beforeAfterScenarioAttributesRun
            = new Stack<BeforeAfterTestAttribute>();

        public ScenarioInvoker(
            IScenario scenario
            , IMessageBus messageBus
            , Type scenarioClass
            , object[] constructorArguments
            , MethodInfo scenarioMethod
            , object[] scenarioMethodArguments
            , IReadOnlyList<BeforeAfterTestAttribute> beforeAfterScenarioAttributes
            , ExceptionAggregator aggregator
            , CancellationTokenSource cancellationTokenSource)
        {
            // TODO: TBD: ditto fluent guards...
            Guard.AgainstNullArgument(nameof(scenario), scenario);
            Guard.AgainstNullArgument(nameof(messageBus), messageBus);
            Guard.AgainstNullArgument(nameof(scenarioClass), scenarioClass);
            Guard.AgainstNullArgument(nameof(scenarioMethod), scenarioMethod);
            Guard.AgainstNullArgument(nameof(beforeAfterScenarioAttributes), beforeAfterScenarioAttributes);
            Guard.AgainstNullArgument(nameof(aggregator), aggregator);
            Guard.AgainstNullArgument(nameof(cancellationTokenSource), cancellationTokenSource);

            this._scenario = scenario;
            this._messageBus = messageBus;
            this._scenarioClass = scenarioClass;
            this._constructorArguments = constructorArguments;
            this._scenarioMethod = scenarioMethod;
            this._scenarioMethodArguments = scenarioMethodArguments;
            this._beforeAfterScenarioAttributes = beforeAfterScenarioAttributes;
            this._aggregator = aggregator;
            this._cancellationTokenSource = cancellationTokenSource;
        }

        public async Task<RunSummary> RunAsync()
        {
            var summary = new RunSummary();
            await this._aggregator.RunAsync(async () =>
            {
                if (!this._cancellationTokenSource.IsCancellationRequested)
                {
                    var testClassInstance = this.CreateScenarioClass();

                    if (!this._cancellationTokenSource.IsCancellationRequested)
                    {
                        await this.BeforeScenarioMethodInvokedAsync();

                        if (!this._cancellationTokenSource.IsCancellationRequested && !this._aggregator.HasExceptions)
                        {
                            summary.Aggregate(await this.InvokeScenarioMethodAsync(testClassInstance));
                        }

                        await this.AfterScenarioMethodInvokedAsync();
                    }

                    if (testClassInstance is IDisposable disposable)
                    {
                        this._timer.Aggregate(() => this._aggregator.Run(disposable.Dispose));
                    }
                }

                summary.Time += this._timer.Total;
            });

            return summary;
        }

        private static string GetStepDisplayName(string scenarioDisplayName, int stepNumber, string stepDisplayText) =>
            string.Format(CultureInfo.InvariantCulture, "{0} [{1}] {2}", scenarioDisplayName
                , stepNumber.ToString("D2", CultureInfo.InvariantCulture), stepDisplayText);

        private object CreateScenarioClass()
        {
            object testClass = null;

            if (!this._scenarioMethod.IsStatic && !this._aggregator.HasExceptions)
            {
                this._timer.Aggregate(() => testClass = Activator.CreateInstance(this._scenarioClass, this._constructorArguments));
            }

            return testClass;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions are collected in the aggregator.")]
        private Task BeforeScenarioMethodInvokedAsync()
        {
            foreach (var beforeAfterAttribute in this._beforeAfterScenarioAttributes)
            {
                try
                {
                    this._timer.Aggregate(() => beforeAfterAttribute.Before(this._scenarioMethod));
                    this._beforeAfterScenarioAttributesRun.Push(beforeAfterAttribute);
                }
                catch (Exception ex)
                {
                    this._aggregator.Add(ex);
                    break;
                }

                if (this._cancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }
            }

            return Task.FromResult(0);
        }

        private Task AfterScenarioMethodInvokedAsync()
        {
            foreach (var beforeAfterAttribute in this._beforeAfterScenarioAttributesRun)
            {
                this._aggregator.Run(() => this._timer.Aggregate(() => beforeAfterAttribute.After(this._scenarioMethod)));
            }

            return Task.FromResult(0);
        }

        private async Task<RunSummary> InvokeScenarioMethodAsync(object scenarioClassInstance)
        {
            var backgroundStepDefinitions = new List<IStepDefinition>();
            var scenarioStepDefinitions = new List<IStepDefinition>();
            await this._aggregator.RunAsync(async () =>
            {
                using (CurrentThread.EnterStepDefinitionContext())
                {
                    foreach (var backgroundMethod in this._scenario.TestCase.TestMethod.TestClass.Class
                        .GetMethods(false)
                        .Where(candidate => candidate.GetCustomAttributes(typeof(BackgroundAttribute)).Any())
                        .Select(method => method.ToRuntimeMethod()))
                    {
                        await this._timer.AggregateAsync(() => backgroundMethod.InvokeAsync(scenarioClassInstance, null));
                    }

                    backgroundStepDefinitions.AddRange(CurrentThread.StepDefinitions);
                }

                using (CurrentThread.EnterStepDefinitionContext())
                {
                    await this._timer.AggregateAsync(() =>
                        this._scenarioMethod.InvokeAsync(scenarioClassInstance, this._scenarioMethodArguments));

                    scenarioStepDefinitions.AddRange(CurrentThread.StepDefinitions);
                }
            });

            var runSummary = new RunSummary { Time = this._timer.Total };
            if (!this._aggregator.HasExceptions)
            {
                runSummary.Aggregate(await this.InvokeStepsAsync(backgroundStepDefinitions, scenarioStepDefinitions));
            }

            return runSummary;
        }

        private async Task<RunSummary> InvokeStepsAsync(ICollection<IStepDefinition> backGroundStepDefinitions
            , ICollection<IStepDefinition> scenarioStepDefinitions)
        {
            var scenarioTypeInfo = this._scenarioClass.GetTypeInfo();
            var filters = scenarioTypeInfo.Assembly.GetCustomAttributes(typeof(Attribute))
                .Concat(scenarioTypeInfo.GetCustomAttributes(typeof(Attribute)))
                .Concat(this._scenarioMethod.GetCustomAttributes(typeof(Attribute)))
                .OfType<IFilter<IStepDefinition>>();

            var summary = new RunSummary();
            string skipReason = null;
            var scenarioTeardowns = new List<Tuple<StepContext, Func<IStepContext, Task>>>();
            var stepNumber = 0;
            foreach (var stepDefinition in filters.Aggregate(
                backGroundStepDefinitions.Concat(scenarioStepDefinitions),
                (current, filter) => filter.Filter(current)))
            {
                stepDefinition.SkipReason = stepDefinition.SkipReason ?? skipReason;

                var stepDisplayName = GetStepDisplayName(this._scenario.DisplayName, ++stepNumber
                    , stepDefinition.DisplayTextFunc?.Invoke(stepDefinition.Text, stepNumber <= backGroundStepDefinitions.Count));

                var step = new StepTest(this._scenario, stepDisplayName);

                // TODO: TBD: is a potential candidate going out of scope and being disposed before it should be...
                // TODO: TBD: should probably be established in a full using block instead of floating free like this...
                var interceptingBus = new DelegatingMessageBus(
                    this._messageBus,
                    message =>
                    {
                        if (message is ITestFailed && stepDefinition.FailureBehavior == RemainingSteps.Skip)
                        {
                            skipReason = $"Failed to execute preceding step: {step.DisplayName}";
                        }
                    });

                var stepContext = new StepContext(step);

                var stepRunner = new StepTestRunner(
                    stepContext
                    , stepDefinition.Body
                    , step
                    , interceptingBus
                    , this._scenarioClass
                    , this._constructorArguments
                    , this._scenarioMethod
                    , this._scenarioMethodArguments
                    , stepDefinition.SkipReason
                    , Array.Empty<BeforeAfterTestAttribute>()
                    , new ExceptionAggregator(this._aggregator)
                    , this._cancellationTokenSource);

                summary.Aggregate(await stepRunner.RunAsync());

                // TODO: TBD: could we use disposable?.Dispose() here?
                var stepTeardowns = stepContext.Disposables
                    .Where(disposable => disposable != null)
                     .Select((Func<IDisposable, Func<IStepContext, Task>>)(disposable => context =>
                     {
                         disposable.Dispose();
                         return Task.FromResult(0);
                     }))
                    .Concat(stepDefinition.Teardowns)
                    .Where(teardown => teardown != null)
                    .Select(teardown => Tuple.Create(stepContext, teardown));

                scenarioTeardowns.AddRange(stepTeardowns);
            }

            if (scenarioTeardowns.Any())
            {
                scenarioTeardowns.Reverse();
                var teardownTimer = new ExecutionTimer();
                var teardownAggregator = new ExceptionAggregator();
                foreach (var teardown in scenarioTeardowns)
                {
                    await Invoker.Invoke(() => teardown.Item2(teardown.Item1), teardownAggregator, teardownTimer);
                }

                summary.Time += teardownTimer.Total;

                if (teardownAggregator.HasExceptions)
                {
                    summary.Failed++;
                    summary.Total++;

                    var stepDisplayName = GetStepDisplayName(this._scenario.DisplayName, ++stepNumber, "(Teardown)");

                    this._messageBus.Queue(new StepTest(this._scenario, stepDisplayName)
                        , test => new TestFailed(test, teardownTimer.Total, null, teardownAggregator.ToException())
                        , this._cancellationTokenSource);
                }
            }

            return summary;
        }
    }
}
