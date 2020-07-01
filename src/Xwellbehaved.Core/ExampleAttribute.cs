using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Xwellbehaved
{
    using Xunit;
    using Xunit.Sdk;

    /// <summary>
    /// Provides example values for a scenario passed as arguments to the scenario method. This
    /// attribute is designed as a synonym of <see cref="InlineDataAttribute"/>, which is the
    /// most commonly used data attribute, but you can also use any type of attribute derived from
    /// <see cref="DataAttribute"/> to provide a data source for a scenario. For instance,
    /// <see cref="InlineDataAttribute"/> or <see cref="MemberDataAttribute"/>.
    /// </summary>
    /// <see cref="DataAttribute"/>
    /// <see cref="InlineDataAttribute"/>
    /// <see cref="MemberDataAttribute"/>
    [DataDiscoverer("Xunit.Sdk.InlineDataDiscoverer", "xunit.core")
        , AttributeUsage(AttributeTargets.Method, AllowMultiple = true)
        , SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Justification = "Following the pattern of Xunit.InlineDataAttribute.")]
    public sealed class ExampleAttribute : DataAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleAttribute"/> class. This attribute
        /// is designed as a synonym of <see cref="InlineDataAttribute"/>, which is the most
        /// commonly used data attribute, but you can also use any type of attribute derived from
        /// <see cref="DataAttribute"/> to provide a data source for a scenario. For instance,
        /// <see cref="InlineDataAttribute"/> or <see cref="MemberDataAttribute"/>.
        /// </summary>
        /// <param name="data">The data values to pass to the scenario.</param>
        /// <see cref="ExampleAttribute"/>
        /// <see cref="DataAttribute"/>
        /// <see cref="InlineDataAttribute"/>
        /// <see cref="MemberDataAttribute"/>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "data", Justification = "Following the pattern of Xunit.InlineDataAttribute.")]
#pragma warning disable IDE0060 // Remove unused parameter
        public ExampleAttribute(params object[] data)
#pragma warning restore IDE0060 // Remove unused parameter
        {
        }

        /// <inheritdoc/>
        public override IEnumerable<object[]> GetData(MethodInfo testMethod) => throw new InvalidOperationException();
    }
}
