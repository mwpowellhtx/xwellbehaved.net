using System.Collections.Generic;
using System.Linq;

namespace Xwellbehaved.Infrastructure
{
    using Xunit;
    using Xunit.Abstractions;

    // TODO: TBD: ditto Xml comments...
    public static class Xunit2Extensions
    {
        public static IEnumerable<IMessageSinkMessage> Run(this Xunit2 runner, IEnumerable<ITestCase> testCases)
        {
            if (!testCases.Any())
            {
                return Enumerable.Empty<IMessageSinkMessage>();
            }

            using (var sink = new SpyMessageSink<ITestCollectionFinished>())
            {
                runner.RunTests(testCases, sink, TestFrameworkOptions.ForExecution());
                sink.Finished.WaitOne();
                return sink.Messages.Select(_ => _);
            }
        }
    }
}
