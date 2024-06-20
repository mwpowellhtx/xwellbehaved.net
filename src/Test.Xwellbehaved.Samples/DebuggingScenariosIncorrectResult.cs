using System;

namespace Xwellbehaved
{
    using Xunit;
    using Xunit.Abstractions;
    using Xunit.Sdk;

    public class DebuggingScenariosIncorrectResult : DebuggingScenarios
    {
        /// <summary>
        /// Action used during the scenario.
        /// </summary>
        private Action Action { get; set; }

        public DebuggingScenariosIncorrectResult(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        public override void Addition(int x, int y, int expected, int actual, Calculator calculator)
        {
            "Let x equal to 1".x(() => x = 1);

            "Let y equal to 2".x(() => y = 2);

            "Let expected equal to 4".x(() => expected = 4);

            "And given calculator".x(() => calculator = new Calculator());

            // TODO: might not be the worst idea to have an 'AssertNotNull<T>() where T : class'
            "When x plus y".x(() => actual = calculator.AssertNotNull().AssertIsType<Calculator>().Add(x, y));

            // Working within the parameters of the override signature, we setup a private Action.
            "Arrange exception action".x(() => this.Action = () => actual.AssertEqual(expected));

            // Because we are expecting Add to yield not equal, so we should see an exception in this instance.
            "Then actual equal to expected".x(() => this.Action.AssertThrows<EqualException>());
        }
    }
}
