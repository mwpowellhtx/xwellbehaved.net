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
        [Fact]
        public void Verify()
        {
            object obj = null;

#pragma warning disable IDE0039 // Use local function
            Action requiresAction = () => Requires.NotNull(obj, nameof(obj));

            Action guardAction = () => Guard.AgainstNullArgument(nameof(obj), obj);

            var guardEx = Assert.Throws<ArgumentNullException>(guardAction);

            var requiresEx = Assert.Throws<ArgumentNullException>(requiresAction);
#pragma warning restore IDE0039 // Use local function

        }
    }
}
