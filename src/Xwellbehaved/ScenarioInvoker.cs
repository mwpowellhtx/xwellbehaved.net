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

#if DEBUG
    using Validation;
#endif

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
            //Guard.AgainstNullArgument(nameof(scenario), scenario);
            //Guard.AgainstNullArgument(nameof(messageBus), messageBus);
            //Guard.AgainstNullArgument(nameof(scenarioClass), scenarioClass);
            //Guard.AgainstNullArgument(nameof(scenarioMethod), scenarioMethod);
            //Guard.AgainstNullArgument(nameof(beforeAfterScenarioAttributes), beforeAfterScenarioAttributes);
            //Guard.AgainstNullArgument(nameof(aggregator), aggregator);
            //Guard.AgainstNullArgument(nameof(cancellationTokenSource), cancellationTokenSource);

#if DEBUG
            scenario.RequiresNotNull(nameof(scenario));
            messageBus.RequiresNotNull(nameof(messageBus));
            scenarioClass.RequiresNotNull(nameof(scenarioClass));
            scenarioMethod.RequiresNotNull(nameof(scenarioMethod));
            beforeAfterScenarioAttributes.RequiresNotNull(nameof(beforeAfterScenarioAttributes));
            aggregator.RequiresNotNull(nameof(aggregator));
            cancellationTokenSource.RequiresNotNull(nameof(cancellationTokenSource));
#endif

            this._scenario = scenario;
            // TODO: TBD: #1 MWP 2020-07-01 11:58:07 AM: but which also beggars the question, should we be cleaning up after IDisposable members like messageBus?
            this._messageBus = messageBus;
            this._scenarioClass = scenarioClass;
            this._constructorArguments = constructorArguments; // TODO: TBD: put fluent validation guards on the arguments?
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

        /* #2 MWP 2020-07-01 12:32:08 PM / for use comparing the Background Method DeclaringType
         * and BaseTypes. */
        /// <summary>
        /// Comparer ensures that <see cref="MethodInfo"/> is ordered from the basest of classes
        /// through the derivedest of classes, in that order.
        /// </summary>
        private class MethodInfoComparer : IComparer<MethodInfo>
        {
            /// <summary>
            /// Returns the Comparison of <paramref name="x"/> with <paramref name="y"/>.
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public int Compare(MethodInfo x, MethodInfo y)
            {
                var lineage = new List<Type> { };

                /* #2 MWP 2020-07-01 12:34:25 PM / netstandard2.0 target is strongly implied by
                 * usage of Type.BaseType; see
                 * https://docs.microsoft.com/en-us/dotnet/api/system.type.basetype; otherwise,
                 * netstandard1.0 would be just fine here. */

                // Capture the hierarchy Type lineage, starting with itself, out to the base type.
                for (var type = y.DeclaringType; type != null; type = type.BaseType)
                {
                    // Remember, in reverse order, should be sufficient to check IndexOf zero being the same type.
                    lineage.Add(type);
                }

                // Compare the indices of the X Method DeclaringType with the lineage of the Y Method DeclaringType.
                var typeIndex = lineage.IndexOf(x.DeclaringType);

                const int isParentClass = -1;
                const int isSameClass = 0;
                const int isChildClass = 1;

                return typeIndex < isSameClass
                    ? isChildClass
                    : typeIndex == isSameClass ? isSameClass : isParentClass;
            }
        }

        private async Task<RunSummary> InvokeScenarioMethodAsync(object scenarioClassInstance)
        {
            var backgroundSteps = new List<IStepDefinition>();
            var scenarioSteps = new List<IStepDefinition>();
            var tearDownSteps = new List<IStepDefinition>();
            await this._aggregator.RunAsync(async () =>
            {
                // We leverage this for both Background and TearDown methods.
                async Task OnDiscoverSupportMethodsAsync<TAttribute>(
                    IScenario scenario, ExecutionTimer timer)
                    where TAttribute : SupportMethodAttribute
                {
                    var methods = scenario.TestCase.TestMethod.TestClass.Class
                        .GetMethods(false)
                        .Where(candidate => candidate.GetCustomAttributes(typeof(TAttribute)).Any())
                        .Select(method => method.ToRuntimeMethod())
                        // #2 MWP 2020-07-01 12:28:03 PM: the rubber meeting the road here.
                        .OrderBy(method => method, new MethodInfoComparer()).ToList();

                    // Thus ensuring correct front to back Background, reverse TearDown.
                    if (typeof(TAttribute) == typeof(TearDownAttribute))
                    {
                        methods.Reverse();
                    }

                    foreach (var method in methods)
                    {
                        /* #4 MWP 2020-07-09 05:47:25 PM / We support seeding default values into
                         * Background methods with parameters. However, this is the extent of what
                         * we can accomplish there. It does not make any sense to cross any Theory
                         * dataRow bridges for Background methods. */
                        var argTypes = method.GetParameters().Select(param => param.ParameterType).ToArray();
                        var args = argTypes.Select(paramType => paramType.GetDefault()).ToArray();

                        var convertedArgs = Reflector.ConvertArguments(args, argTypes);
                        convertedArgs = convertedArgs.Any() ? convertedArgs : null;

                        await timer.AggregateAsync(() => method.InvokeAsync(scenarioClassInstance, convertedArgs));
                    }
                }

                using (CurrentThread.EnterStepDefinitionContext())
                {
                    await OnDiscoverSupportMethodsAsync<BackgroundAttribute>(this._scenario, this._timer);

                    backgroundSteps.AddRange(CurrentThread.StepDefinitions);
                }

                using (CurrentThread.EnterStepDefinitionContext())
                {
                    await this._timer.AggregateAsync(() => this._scenarioMethod.InvokeAsync(scenarioClassInstance, this._scenarioMethodArguments));

                    scenarioSteps.AddRange(CurrentThread.StepDefinitions);
                }

                // MWP Sun, 06 Dec 2020 11:00:56 AM / Rinse and repeat Background, except for TearDown...
                using (CurrentThread.EnterStepDefinitionContext())
                {
                    await OnDiscoverSupportMethodsAsync<TearDownAttribute>(this._scenario, this._timer);

                    tearDownSteps.AddRange(CurrentThread.StepDefinitions);
                }
            });

            var runSummary = new RunSummary { Time = this._timer.Total };
            if (!this._aggregator.HasExceptions)
            {
                // Assumes Scenario for StepDefinitionType.
                backgroundSteps.ForEach(x => x.StepDefinitionType = StepType.Background);
                tearDownSteps.ForEach(x => x.StepDefinitionType = StepType.TearDown);
                runSummary.Aggregate(await this.InvokeStepsAsync(backgroundSteps, scenarioSteps, tearDownSteps));
            }

            return runSummary;
        }

        /// <summary>
        /// Invokes the Steps given Background, Scenario, and TearDown steps.
        /// </summary>
        /// <param name="backgroundSteps"></param>
        /// <param name="scenarioSteps"></param>
        /// <param name="tearDownSteps"></param>
        /// <returns></returns>
        private async Task<RunSummary> InvokeStepsAsync(ICollection<IStepDefinition> backgroundSteps
            , ICollection<IStepDefinition> scenarioSteps, ICollection<IStepDefinition> tearDownSteps)
        {
            var scenarioTypeInfo = this._scenarioClass.GetTypeInfo();
            var filters = scenarioTypeInfo.Assembly.GetCustomAttributes(typeof(Attribute))
                .Concat(scenarioTypeInfo.GetCustomAttributes(typeof(Attribute)))
                .Concat(this._scenarioMethod.GetCustomAttributes(typeof(Attribute)))
                .OfType<IFilter<IStepDefinition>>();

            var summary = new RunSummary();
            string skipReason = null;
            var scenarioRollbacks = new List<(StepContext context, Func<IStepContext, Task> callback)>();
            var stepNumber = 0;
            foreach (var stepDefinition in filters.Aggregate(backgroundSteps.Concat(scenarioSteps).Concat(tearDownSteps)
                , (current, filter) => filter.Filter(current)))
            {
                stepDefinition.SkipReason = stepDefinition.SkipReason ?? skipReason;

                var stepDisplayName = GetStepDisplayName(this._scenario.DisplayName
                    , ++stepNumber
                    , stepDefinition.OnDisplayText?.Invoke(
                        stepDefinition.Text, stepDefinition.StepDefinitionType)
                );

                var step = new StepTest(this._scenario, stepDisplayName);

                // #1 MWP 2020-07-01 11:56:45 AM: After testing, this should be fine, and better behaved we think.
                using (var interceptingBus = new DelegatingMessageBus(
                    this._messageBus
                    , message =>
                    {
                        if (message is ITestFailed && stepDefinition.FailureBehavior == RemainingSteps.Skip)
                        {
                            skipReason = $"Failed to execute preceding step: {step.DisplayName}";
                        }
                    })
                )
                {
                    var stepContext = new StepContext(step);

                    // TODO: TBD: #1 MWP 2020-07-01 11:57:26 AM: it is an xUnit thing, could possibly be IDisposable itself...
                    // TODO: TBD: including assumed ownership of the IDisposable messageBus, for instance, TODO: TBD: but that is outside the scope of xWellBehaved...
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
                    var stepRollbacks = stepContext.Disposables
                        .Where(disposable => disposable != null)
                         .Select((Func<IDisposable, Func<IStepContext, Task>>)(disposable => context =>
                         {
                             disposable.Dispose();
                             return Task.FromResult(0);
                         }))
                        .Concat(stepDefinition.Rollbacks)
                        .Where(onRollback => onRollback != null)
                        .Select(onRollback => (stepContext, onRollback));

                    scenarioRollbacks.AddRange(stepRollbacks);
                }
            }

            if (scenarioRollbacks.Any())
            {
                scenarioRollbacks.Reverse();
                var rollbackTimer = new ExecutionTimer();
                var rollbackAggregator = new ExceptionAggregator();

                // "Teardowns" not to be confused with TearDown versus Background.
                foreach (var (context, onRollback) in scenarioRollbacks)
                {
                    await Invoker.Invoke(() => onRollback.Invoke(context), rollbackAggregator, rollbackTimer);
                }

                summary.Time += rollbackTimer.Total;

                if (rollbackAggregator.HasExceptions)
                {
                    summary.Failed++;
                    summary.Total++;

                    var stepDisplayName = GetStepDisplayName(this._scenario.DisplayName, ++stepNumber, $"({StepType.Rollback})");

                    this._messageBus.Queue(new StepTest(this._scenario, stepDisplayName)
                        , test => new TestFailed(test, rollbackTimer.Total, null, rollbackAggregator.ToException())
                        , this._cancellationTokenSource);
                }
            }

            return summary;
        }
    }
}
