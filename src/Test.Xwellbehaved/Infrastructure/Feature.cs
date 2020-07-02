using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace Xwellbehaved.Infrastructure
{
    using Xunit;
    using Xunit.Abstractions;

    // TODO: TBD: ditto Xml comments...
    public abstract class Feature : IDisposable
    {
        private readonly IList<Xunit2> _runners = new List<Xunit2>();

        ~Feature()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public TMessage[] Run<TMessage>(Assembly assembly, string collectionName)
            where TMessage : IMessageSinkMessage
            => this.Run(assembly, collectionName).OfType<TMessage>().ToArray();

        public TMessage[] Run<TMessage>(Type feature)
            where TMessage : IMessageSinkMessage
            => this.Run(feature).OfType<TMessage>().ToArray();

        public TMessage[] Run<TMessage>(Type feature, string traitName, string traitValue)
            where TMessage : IMessageSinkMessage
            => this.Run(feature, traitName, traitValue).OfType<TMessage>().ToArray();

        public IMessageSinkMessage[] Run(Assembly assembly, string collectionName)
        {
            var runner = this.CreateRunner(assembly.GetLocalCodeBase());
            var testCase = runner.Find(collectionName);
            return runner.Run(testCase).ToArray();
        }

        public IMessageSinkMessage[] Run(Type feature)
        {
            var runner = this.CreateRunner(feature.GetTypeInfo().Assembly.GetLocalCodeBase());
            var testCases = runner.Find(feature);
            var testCaseResults = runner.Run(testCases).ToArray();
            return testCaseResults;
        }

        public IMessageSinkMessage[] Run(Type feature, string traitName, string traitValue)
        {
            var runner = this.CreateRunner(feature.GetTypeInfo().Assembly.GetLocalCodeBase());

            var testCases = runner.Find(feature).ToArray();

            var traitTestCases = testCases.Where(testCase =>
                testCase.Traits.TryGetValue(traitName, out var values)
                    && values.Contains(traitValue)).ToArray();

            return runner.Run(traitTestCases).ToArray();
        }

        /// <summary>
        /// Gets the FeatureArtifacts in play during the resolution of the various tests, Facts,
        /// Theories, Scenarios, etc.
        /// </summary>
        internal IList<Type> FeatureArtifacts { get; } = new List<Type> { };

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Exception exception = null;
                foreach (var runner in this._runners.Reverse())
                {
                    try
                    {
                        runner.Dispose();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                }

                if (exception != null)
                {
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }

                /* We must do this on disposal, because to do it midstream during resolution of a
                 * test case is inappropriate, and the file or files, or other artifacts, may, and
                 * quite likely are, still in use. */

                foreach (var featureArtifact in this.FeatureArtifacts)
                {
                    featureArtifact.ClearTestEvents();
                }
            }
        }

        private Xunit2 CreateRunner(string assemblyFileName)
        {
            this._runners.Add(new Xunit2(AppDomainSupport.IfAvailable, new NullSourceInformationProvider(), assemblyFileName));
            return this._runners.Last();
        }
    }
}
