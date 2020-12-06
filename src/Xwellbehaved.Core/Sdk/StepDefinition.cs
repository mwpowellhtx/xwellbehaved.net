using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Xwellbehaved.Sdk
{
    [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Step", Justification = "By design.")]
    internal class StepDefinition : IStepDefinition
    {
        public string Text { get; set; }

        /// <inheritdoc/>
        public StepType StepDefinitionType { get; set; } = StepType.Scenario;

        public Func<IStepContext, Task> Body { get; set; }

        public ICollection<Func<IStepContext, Task>> Teardowns { get; } = new List<Func<IStepContext, Task>>();

        public string SkipReason { get; set; }

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
        public GetStepDisplayText OnDisplayTextCallback { get; set; } = DefaultOnDisplayText;

        public IStepDefinition Skip(string reason)
        {
            this.SkipReason = reason;
            return this;
        }

        public IStepDefinition Teardown(Func<IStepContext, Task> action)
        {
            if (action != null)
            {
                this.Teardowns.Add(action);
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
        public IStepDefinition OnDisplayText(GetStepDisplayText onDisplayTextCallback)
        {
            this.OnDisplayTextCallback = onDisplayTextCallback;
            return this;
        }

        IStepBuilder IStepBuilder.Skip(string reason) => this.Skip(reason);

        IStepBuilder IStepBuilder.Teardown(Func<IStepContext, Task> action) => this.Teardown(action);

        IStepBuilder IStepBuilder.OnFailure(RemainingSteps behavior) => this.OnFailure(behavior);
    }
}
