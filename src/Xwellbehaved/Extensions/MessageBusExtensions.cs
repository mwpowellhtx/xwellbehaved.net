using System;
using System.Threading;

namespace Xwellbehaved.Execution.Extensions
{

#if DEBUG
    using Validation;
#endif

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
            //Guard.AgainstNullArgument(nameof(messageBus), messageBus);
            //Guard.AgainstNullArgument(nameof(cancellationTokenSource), cancellationTokenSource);

#if DEBUG
            messageBus = messageBus.RequiresNotNull(nameof(messageBus));
            cancellationTokenSource = cancellationTokenSource.RequiresNotNull(nameof(cancellationTokenSource));
#endif

            if (!messageBus.QueueMessage(new TestStarting(test)))
            {
                cancellationTokenSource.Cancel();
            }
            else
            {
                //Guard.AgainstNullArgument(nameof(createTestResultMessage), createTestResultMessage);

#if DEBUG
                createTestResultMessage = createTestResultMessage.RequiresNotNull(nameof(createTestResultMessage));
#endif

                var message = createTestResultMessage.Invoke(test);
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
