using System;
using System.Linq;
using System.Threading.Tasks;

namespace Xwellbehaved
{
    using FluentAssertions;
    using Xunit.Abstractions;
    using Xwellbehaved.Infrastructure;
    using Xwellbehaved.Sdk;

    public class AsyncScenarioFeature : Feature
    {
        private static class AsyncScenarioThatThrowsAfterYielding
        {
            [Scenario]
            public static async Task Scenario()
            {
                "Given".x(() => { });

                await Task.Yield();
                throw new InvalidOperationException("I yielded before this.");
            }
        }

        [Scenario]
        public void AsyncScenario(Type feature, ITestResultMessage[] results)
        {
            "Given an async scenario that throws after yielding".x(
                () => feature = typeof(AsyncScenarioThatThrowsAfterYielding));

            "When I run the scenario".x(
                () => results = this.Run<ITestResultMessage>(feature));

            "Then the scenario fails".x(
                () => results.Single().Should().BeAssignableTo<ITestFailed>());

            "And the exception is the exception thrown after the yield".x(
                () => results.Cast<ITestFailed>().Single().Messages.Single().Should().Be("I yielded before this."));
        }

        [Scenario]
        public void NullStepBody() =>
            "Given a null body".x((Func<Task>)null);

        [Scenario]
        public void NullContextualStepBody() =>
            "Given a null body".x((Func<IStepContext, Task>)null);
    }
}
