using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Xwellbehaved.Execution
{

#if DEBUG
    using Validation;
#endif

    using Xunit.Abstractions;
    using Xunit.Sdk;

    /// <summary>
    /// Implementation of the <see cref="IXunitTestCaseDiscoverer"/> that supports finding
    /// test cases on methods decorated with the <see cref="ScenarioAttribute"/>.
    /// </summary>
    public class ScenarioDiscoverer : TheoryDiscoverer
    {
        public ScenarioDiscoverer(IMessageSink diagnosticMessageSink)
            : base(diagnosticMessageSink)
        {
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Factory method.")]
        public override IEnumerable<IXunitTestCase> Discover(
            ITestFrameworkDiscoveryOptions discoveryOptions
            , ITestMethod testMethod
            , IAttributeInfo factAttribute)
        {
            //Guard.AgainstNullArgument(nameof(discoveryOptions), discoveryOptions);

#if DEBUG
            discoveryOptions = discoveryOptions.RequiresNotNull(nameof(discoveryOptions));
#endif

            yield return new ScenarioOutlineTestCase(
                this.DiagnosticMessageSink
                , discoveryOptions.MethodDisplayOrDefault()
                , discoveryOptions.MethodDisplayOptionsOrDefault()
                , testMethod);
        }
    }
}
