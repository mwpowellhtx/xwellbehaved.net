using System;
using System.Diagnostics.CodeAnalysis;

namespace Xwellbehaved
{
    /// <summary>
    /// Applied to a method to indicate a background for each scenario defined in the same feature
    /// class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)
        , SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes", Justification = "Designed for extensibility.")
        , IgnoreXunitAnalyzersRule1013]
    public abstract class SupportMethodAttribute : Attribute
    {
        [AttributeUsage(AttributeTargets.Class)]
        private class IgnoreXunitAnalyzersRule1013Attribute : Attribute
        {
        }
    }
}
