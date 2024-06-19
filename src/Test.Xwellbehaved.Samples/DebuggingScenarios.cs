namespace Xwellbehaved
{
    using Xunit;
    using Xunit.Abstractions;

    public class DebuggingScenarios : CalculatorFeature
    {
        public DebuggingScenarios(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [Scenario]
        public virtual void Addition(int x, int y, int expected, int actual, Calculator calculator)
        {
            "Let x equal to 1".x(() => x = 1);

            "Let y equal to 2".x(() => y = 2);

            "Let expected equal to 3".x(() => expected = 3);

            "And given calculator".x(() => calculator = new Calculator());

            // TODO: ditto generic AssertNotNull
            "When x plus y".x(() => actual = calculator.AssertNotNull().AssertIsType<Calculator>().Add(x, y));

            "Then actual equal to expected".x(() => actual.AssertEqual(expected));
        }
    }
}
