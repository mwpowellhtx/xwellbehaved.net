using System;
using System.Threading.Tasks;

namespace Xwellbehaved
{
    using Xwellbehaved.Sdk;

    /// <summary>
    /// Provides extension methods for building steps.
    /// </summary>
    public static class IStepBuilderExtensions
    {
        /// <summary>
        /// Declares a teardown action, relating to this step or previous steps, which will be
        /// executed after all steps in the current scenario have been executed.
        /// </summary>
        /// <param name="stepBuilder">The step builder.</param>
        /// <param name="onTeardown">The teardown callback.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        [Obsolete("We are moving away from tear down verbiage in favor of rollback so as not to confuse with Tear Down decoration")]
        public static IStepBuilder Teardown(this IStepBuilder stepBuilder, Action onTeardown) => stepBuilder.Rollback(onTeardown);

        /// <summary>
        /// Declares a teardown action, relating to this step or previous steps, which will be
        /// executed after all steps in the current scenario have been executed.
        /// </summary>
        /// <param name="stepBuilder">The step builder.</param>
        /// <param name="onTeardown">The teardown callback.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        [Obsolete("We are moving away from tear down verbiage in favor of rollback so as not to confuse with Tear Down decoration")]
        public static IStepBuilder Teardown(this IStepBuilder stepBuilder, Action<IStepContext> onTeardown) => stepBuilder.Rollback(onTeardown);

        /// <summary>
        /// Declares a teardown action, relating to this step or previous steps, which will be
        /// executed after all steps in the current scenario have been executed.
        /// </summary>
        /// <param name="stepBuilder">The step builder.</param>
        /// <param name="onTeardown">The teardown callback.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        [Obsolete("We are moving away from tear down verbiage in favor of rollback so as not to confuse with Tear Down decoration")]
        public static IStepBuilder Teardown(this IStepBuilder stepBuilder, Func<Task> onTeardown) => stepBuilder.Rollback(onTeardown);

        /// <summary>
        /// Declares a rollback action, relating to this step or previous steps, which will be
        /// executed after all steps in the current scenario have been executed.
        /// </summary>
        /// <param name="stepBuilder">The step builder.</param>
        /// <param name="onRollback">The rollback callback.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        public static IStepBuilder Rollback(this IStepBuilder stepBuilder, Action onRollback) =>
            onRollback == null
                ? stepBuilder
                : stepBuilder.Rollback(context => onRollback());

        /// <summary>
        /// Declares a rollback action, relating to this step or previous steps, which will be
        /// executed after all steps in the current scenario have been executed.
        /// </summary>
        /// <param name="stepBuilder">The step builder.</param>
        /// <param name="onRollback">The rollback callback.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        public static IStepBuilder Rollback(this IStepBuilder stepBuilder, Action<IStepContext> onRollback) =>
            onRollback == null
                ? stepBuilder
                : stepBuilder?.Rollback(context =>
                {
                    onRollback(context);
                    return Task.FromResult(0);
                });

        /// <summary>
        /// Declares a rollback action, relating to this step or previous steps, which will be
        /// executed after all steps in the current scenario have been executed.
        /// </summary>
        /// <param name="stepBuilder">The step builder.</param>
        /// <param name="onRollback">The rollback callback.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        public static IStepBuilder Rollback(this IStepBuilder stepBuilder, Func<Task> onRollback) =>
            onRollback == null
                ? stepBuilder
                : stepBuilder?.Rollback(context => onRollback());
    }
}
