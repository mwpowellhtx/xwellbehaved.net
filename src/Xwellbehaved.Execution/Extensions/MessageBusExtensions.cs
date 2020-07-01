using System;
using System.Threading;

namespace Xwellbehaved.Execution.Extensions
{
    using Xunit.Abstractions;
    using Xunit.Sdk;

    // TODO: TBD: can and probably should comment in Xml comments...
    internal static class MessageBusExtensions
    {
        public static void Queue(
            this IMessageBus messageBus
            , ITest test
            , Func<ITest, IMessageSinkMessage> createTestResultMessage
            , CancellationTokenSource cancellationTokenSource)
        {
            // TODO: TBD: ditto fluent guards...
            Guard.AgainstNullArgument(nameof(messageBus), messageBus);
            Guard.AgainstNullArgument(nameof(createTestResultMessage), createTestResultMessage);
            Guard.AgainstNullArgument(nameof(cancellationTokenSource), cancellationTokenSource);

            if (!messageBus.QueueMessage(new TestStarting(test)))
            {
                cancellationTokenSource.Cancel();
            }
            else
            {
                if (!messageBus.QueueMessage(createTestResultMessage(test)))
                {
                    cancellationTokenSource.Cancel();
                }
            }

            if (!messageBus.QueueMessage(new TestFinished(test, 0, null)))
            {
                cancellationTokenSource.Cancel();
            }
        }
    }
}
