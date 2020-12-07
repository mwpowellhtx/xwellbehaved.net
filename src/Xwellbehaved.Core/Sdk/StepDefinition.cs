using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Xwellbehaved.Sdk
{
    /// <inheritdoc cref="IStepDefinition"/>
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Step", Justification = "By design.")]
    internal class StepDefinition : IStepDefinition
    {
        /// <inheritdoc/>
        public string Text { get; set; }

        /// <inheritdoc/>
        public StepType StepDefinitionType { get; set; } = StepType.Scenario;

        /// <inheritdoc/>
        public Func<IStepContext, Task> Body { get; set; }

        /// <inheritdoc/>
        public ICollection<Func<IStepContext, Task>> Rollbacks { get; } = new List<Func<IStepContext, Task>>();

        /// <inheritdoc/>
        public ICollection<Func<IStepContext, Task>> Teardowns => this.Rollbacks;

        /// <inheritdoc/>
        public string SkipReason { get; set; }

        /// <inheritdoc/>
        public RemainingSteps FailureBehavior { get; set; }

        /// <summary>
        /// The Default OnDisplayTextCallback Callback
        /// </summary>
        /// <param name="stepText"></param>
        /// <param name="stepDefinitionType"></param>
        /// <returns></returns>
        private static string DefaultOnDisplayText(string stepText, StepType stepDefinitionType) =>
            $"({stepDefinitionType}): {stepText}";

        /// <inheritdoc/>
        public GetStepDisplayText OnDisplayText { get; set; } = DefaultOnDisplayText;

        /// <inheritdoc/>
        public IStepDefinition Skip(string reason)
        {
            this.SkipReason = reason;
            return this;
        }

        /// <inheritdoc/>
        [Obsolete("See notes concerning IStepBuilder.Teardown and IStepDefinition.Teardowns")]
        public IStepDefinition Teardown(Func<IStepContext, Task> onTeardown) => this.Rollback(onTeardown);

        /// <inheritdoc/>
        public IStepDefinition Rollback(Func<IStepContext, Task> onRollback)
        {
            if (onRollback != null)
            {
                this.Rollbacks.Add(onRollback);
            }

            return this;
        }

        /// <inheritdoc/>
        public IStepDefinition OnFailure(RemainingSteps behavior)
        {
            this.FailureBehavior = behavior;
            return this;
        }

        /// <inheritdoc/>
        public IStepDefinition ConfigureDisplayText(GetStepDisplayText onDisplayText)
        {
            this.OnDisplayText = onDisplayText;
            return this;
        }

        /// <inheritdoc/>
        IStepBuilder IStepBuilder.Skip(string reason) => this.Skip(reason);

        /// <inheritdoc/>
        [Obsolete("See notes concerning IStepDefinition.Teardown")]
        IStepBuilder IStepBuilder.Teardown(Func<IStepContext, Task> onTeardown) => this.Rollback(onTeardown);

        /// <inheritdoc/>
        IStepBuilder IStepBuilder.Rollback(Func<IStepContext, Task> onRollback) => this.Rollback(onRollback);

        /// <inheritdoc/>
        IStepBuilder IStepBuilder.OnFailure(RemainingSteps behavior) => this.OnFailure(behavior);
    }
}
