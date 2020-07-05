using System;

namespace Xwellbehaved
{

#if DEBUG
    using Validation;
#endif

    using Xwellbehaved.Sdk;

    /// <summary>
    /// <see cref="IDisposable"/> extensions.
    /// </summary>
    public static class DisposableExtensions
    {
        /// <summary>
        /// Immediately registers the <see cref="IDisposable"/> object for disposal after all
        /// steps in the current scenario have been executed.
        /// </summary>
        /// <typeparam name="T">The <see cref="IDisposable"/> object.</typeparam>
        /// <param name="disposable">The object to be disposed.</param>
        /// <param name="stepContext">The execution context for the current step.</param>
        /// <returns>The object.</returns>
        public static T Using<T>(this T disposable, IStepContext stepContext)
            where T : IDisposable
        {
            //Guard.AgainstNullArgument(nameof(stepContext), stepContext);
            //stepContext.Using(disposable);

#if DEBUG
            //// TODO: TBD: not sure just what exactly is going on with this...
            //Validation.Requires.NotNull(stepContext, nameof(stepContext)).Using(disposable);

            stepContext.RequiresNotNull(nameof(stepContext)).Using(disposable);
#else

            // Which, we "do", in DEBUG mode.
#pragma warning disable CA1062 // ...validate parameter 'name' is non-null before using it...
            stepContext.Using(disposable);
#pragma warning restore CA1062 // ...validate parameter 'name' is non-null before using it...

#endif

            return disposable;
        }
    }
}
