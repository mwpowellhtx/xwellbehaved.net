using System;
using System.Threading;

namespace Xwellbehaved.Execution.Extensions
{
    using Validation;
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
            messageBus = messageBus.RequiresNotNull(nameof(messageBus));
            cancellationTokenSource = cancellationTokenSource.RequiresNotNull(nameof(cancellationTokenSource));

            if (!messageBus.QueueMessage(new TestStarting(test)))
            {
                cancellationTokenSource.Cancel();
            }
            else
            {
                var message = createTestResultMessage.RequiresNotNull(nameof(createTestResultMessage)).Invoke(test);
                if (!messageBus.QueueMessage(message))
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
