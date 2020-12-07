using System;
using System.Threading.Tasks;

namespace Xwellbehaved.Sdk
{
    /// <summary>
    /// Provides methods for building steps.
    /// </summary>
    /// <remarks>This is the type returned from <code>String.x()</code>.</remarks>
    public interface IStepBuilder
    {
        /// <summary>
        /// Indicates that the step will not be executed.
        /// </summary>
        /// <param name="reason">The reason for not executing the step.</param>
        /// <returns>An instance of <see cref="IStepBuilder"/>.</returns>
        /// <remarks>If the <paramref name="reason"/> is <c>null</c> then the step will not be skipped.</remarks>
        IStepBuilder Skip(string reason);

        /// <summary>
        /// Declares a teardown action (related to this step and/or previous steps) which will be executed
        /// after all steps in the current scenario have been executed.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        [Obsolete("We are moving away from this as the syntax to avoid confusion with TearDown annotations")]
        IStepBuilder Teardown(Func<IStepContext, Task> action);

        /// <summary>
        /// Declares a rollback <paramref name="onRollback"/> <see cref="Func{T, TResult}"/>
        /// related to this step or previous steps that will be executed after all steps in
        /// the current scenario have been executed.
        /// </summary>
        /// <param name="onRollback"></param>
        /// <returns></returns>
        IStepBuilder Rollback(Func<IStepContext, Task> onRollback);

        /// <summary>
        /// Defines the behavior of remaining steps if this step fails.
        /// </summary>
        /// <param name="behavior">The behavior of remaining steps.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        IStepBuilder OnFailure(RemainingSteps behavior);
    }
}
