using System;
using System.Diagnostics.CodeAnalysis;

namespace Xwellbehaved
{
    /// <summary>
    /// Applied to a method to indicate a background for each scenario defined in the same feature
    /// class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)
        , SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Designed for extensibility.")]
    public abstract class SupportMethodAttribute : Attribute
    {
    }
}
