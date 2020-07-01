using System;
using System.Collections.Generic;

namespace Xwellbehaved.Execution
{
    using Xwellbehaved.Sdk;

    /// <inheritdoc/>
    public class StepContext : IStepContext
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        public StepContext(IStep step)
        {
            this.Step = step;
        }

        /// <inheritdoc/>
        public IStep Step { get; }

        /// <inheritdoc/>
        public IReadOnlyList<IDisposable> Disposables => this._disposables;

        /// <inheritdoc/>
        public IStepContext Using(IDisposable disposable)
        {
            if (disposable != null)
            {
                this._disposables.Add(disposable);
            }

            return this;
        }
    }
}
