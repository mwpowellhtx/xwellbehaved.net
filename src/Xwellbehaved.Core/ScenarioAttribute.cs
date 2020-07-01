using System;
using System.Diagnostics.CodeAnalysis;

namespace Xwellbehaved
{
    using Xunit;
    using Xunit.Sdk;

    /// <summary>
    /// Applied to a method to indicate the definition of a scenario. A scenario can also be fed
    /// examples from a data source, mapping to parameters on the scenario method. If the data
    /// source contains multiple rows, then the scenario method is executed multiple times
    /// (once with each data row). Examples can be fed to the scenario by applying one or
    /// more instances of <see cref="ExampleAttribute"/> or any other attribute deriving from
    /// <see cref="DataAttribute"/>. For instance, <see cref="InlineDataAttribute"/> or
    /// <see cref="MemberDataAttribute"/>.
    /// </summary>
    /// <see cref="ExampleAttribute"/>
    /// <see cref="DataAttribute"/>
    /// <see cref="InlineDataAttribute"/>
    /// <see cref="MemberDataAttribute"/>
    [XunitTestCaseDiscoverer("Xwellbehaved.Execution.ScenarioDiscoverer", "Xwellbehaved.Execution")
        , AttributeUsage(AttributeTargets.Method)
        , SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Designed for extensibility.")]
    public class ScenarioAttribute : FactAttribute
    {
    }
}
