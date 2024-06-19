using System;
using System.Threading.Tasks;

namespace Xwellbehaved
{
    using Xwellbehaved.Sdk;

    /// <summary>
    /// Provides access to step definition methods.
    /// </summary>
    public static class StringExtensions
    {

#pragma warning disable IDE1006 // Naming Styles
        private static Func<IStepContext, Task> NullBodyCallback => null;

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        public static IStepBuilder x(this string text, Action body)
        {
            var stepDef = new StepDefinition
            {
                Text = text,
                Body = body == null ? NullBodyCallback : c =>
                {
                    body();
                    return Task.FromResult(0);
                },
            };

            CurrentThread.Add(stepDef);
            return stepDef;
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        public static IStepBuilder x(this string text, Action<IStepContext> body)
        {
            var stepDef = new StepDefinition
            {
                Text = text,
                Body = body == null ? NullBodyCallback : c =>
                {
                    body(c);
                    return Task.FromResult(0);
                },
            };

            CurrentThread.Add(stepDef);
            return stepDef;
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        public static IStepBuilder x(this string text, Func<Task> body)
        {
            var stepDef = new StepDefinition
            {
                Text = text,
                Body = body == null ? NullBodyCallback : c => body(),
            };

            CurrentThread.Add(stepDef);
            return stepDef;
        }

        /// <summary>
        /// Defines a step in the current scenario.
        /// </summary>
        /// <param name="text">The step text.</param>
        /// <param name="body">The action that will perform the step.</param>
        /// <returns>
        /// An instance of <see cref="IStepBuilder"/>.
        /// </returns>
        public static IStepBuilder x(this string text, Func<IStepContext, Task> body)
        {
            var stepDef = new StepDefinition { Text = text, Body = body };
            CurrentThread.Add(stepDef);
            return stepDef;
        }
    }
}
