using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Xwellbehaved
{
    using Xunit.Sdk;

    /// <summary>
    /// Base attribute which indicates a scenario method interception, allowing code to be run
    /// before and after the scenario is run.
    /// </summary>
    /// <remarks>
    /// This type derives trivially from <see cref="BeforeAfterTestAttribute"/>. It exists
    /// purely to convey more clearly that
    /// <see cref="BeforeAfterTestAttribute.Before(MethodInfo)"/> and
    /// <see cref="BeforeAfterTestAttribute.After(MethodInfo)"/> will be run before and after
    /// the scenario, rather than before and after each step in the scenario.
    /// </remarks>
    /// <see cref="BeforeAfterTestAttribute"/>
    /// <see cref="BeforeAfterTestAttribute.Before(MethodInfo)"/>
    /// <see cref="BeforeAfterTestAttribute.After(MethodInfo)"/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)
        , SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Designed for extensibility.")]
    public abstract class BeforeAfterScenarioAttribute : BeforeAfterTestAttribute
    {
    }
}
