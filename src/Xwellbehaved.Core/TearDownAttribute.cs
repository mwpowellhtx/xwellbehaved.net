using System;

namespace Xwellbehaved
{
    /// <summary>
    /// Applied to a method to indicate a tear down for each scenario defined in the same feature
    /// class.
    /// </summary>
    [IgnoreXunitAnalyzersRule1013]
    public class TearDownAttribute : SupportMethodAttribute
    {
        [AttributeUsage(AttributeTargets.Class)]
        private class IgnoreXunitAnalyzersRule1013Attribute : Attribute
        {
        }
    }
}
