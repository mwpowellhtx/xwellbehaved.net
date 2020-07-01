using System;
using System.Collections.Generic;
using System.Threading;

namespace Xwellbehaved.Infrastructure
{
    using Xunit.Abstractions;
    using Xunit.Sdk;

    // TODO: TBD: ditto Xml comments...
    /// <inheritdoc cref="IMessageSink"/>
    /// <typeparam name="TFinalMessage"></typeparam>
    public sealed class SpyMessageSink<TFinalMessage> : LongLivedMarshalByRefObject, IMessageSink, IDisposable
    {
        /// <inheritdoc/>
        public ManualResetEvent Finished { get; } = new ManualResetEvent(initialState: false);

        /// <inheritdoc/>
        public IList<IMessageSinkMessage> Messages { get; } = new List<IMessageSinkMessage>();

        /// <inheritdoc/>
        public void Dispose() => this.Finished.Dispose();

        /// <inheritdoc/>
        public bool OnMessage(IMessageSinkMessage message)
        {
            this.Messages.Add(message);

            if (message is TFinalMessage)
            {
                this.Finished.Set();
            }

            return true;
        }
    }
}
