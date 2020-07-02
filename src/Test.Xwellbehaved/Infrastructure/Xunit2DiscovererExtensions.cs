using System;
using System.Collections.Generic;
using System.Linq;

namespace Xwellbehaved.Infrastructure
{
    using Xunit;
    using Xunit.Abstractions;

    // TODO: TBD: ditto Xml comments...
    public static class Xunit2DiscovererExtensions
    {
        public static IEnumerable<ITestCase> Find(this Xunit2Discoverer discoverer, string collectionName)
        {

#pragma warning disable IDE0063 // 'using' statement can be simplified
            using (var sink = new SpyMessageSink<IDiscoveryCompleteMessage>())
            {
                discoverer.Find(false, sink, TestFrameworkOptions.ForDiscovery());
                sink.Finished.WaitOne();
                return sink.Messages.OfType<ITestCaseDiscoveryMessage>()
                    .Select(message => message.TestCase)
                    .Where(message => message.TestMethod.TestClass.TestCollection.DisplayName == collectionName)
                    .ToArray();
            }
#pragma warning restore IDE0063 // 'using' statement can be simplified

        }

        public static IEnumerable<ITestCase> Find(this Xunit2Discoverer discoverer, Type type)
        {

#pragma warning disable IDE0063 // 'using' statement can be simplified
            using (var sink = new SpyMessageSink<IDiscoveryCompleteMessage>())
            {
                discoverer.Find(type.FullName, false, sink, TestFrameworkOptions.ForDiscovery());
                sink.Finished.WaitOne();
                return sink.Messages.OfType<ITestCaseDiscoveryMessage>()
                    .Select(message => message.TestCase).ToArray();
            }
#pragma warning restore IDE0063 // 'using' statement can be simplified

        }
    }
}
