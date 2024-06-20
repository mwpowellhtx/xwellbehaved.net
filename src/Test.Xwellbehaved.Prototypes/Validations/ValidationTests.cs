using System;

namespace Xwellbehaved.Prototypes.Validations
{
    using Validation;
    using Xunit;

    /// <summary>
    /// This was kind of a no brainer, we just want to see what the potential differences might be
    /// between one method of performing guards and the next. When using <see cref="Guard"/>,
    /// things are working fine, for whatever reason. However, when we try to substitute in for
    /// it, such as with ones based on <see cref="Requires"/>, things all of a sudden fall apart
    /// and no longer work. Tests, behaviors, etc, no longer run. The <see cref="FactAttribute"/>
    /// based tests do, but the <see cref="ScenarioAttribute"/> based behaviors do not.
    /// </summary>
    /// <see cref="!:https://github.com/xbehave/xbehave.net/issues/665">Q: What&apos;s the difference between LiteGuard.Source and Validation</see>
    public class ValidationTests
    {
        /// <summary>
        /// 
        /// </summary>
        [Fact]
        public void Verify()
        {
            // Took the opportunity to update for language features.
            var obj = default(object);

            // Although, fundamentally, csharp and dotnet remain pretty consistent.
            void RequiresAction() => Requires.NotNull(obj, nameof(obj));

            void GuardAction() => Guard.AgainstNullArgument(nameof(obj), obj);

            var guardEx = Assert.Throws<ArgumentNullException>(GuardAction);

            var requiresEx = Assert.Throws<ArgumentNullException>(RequiresAction);
        }
    }
}
