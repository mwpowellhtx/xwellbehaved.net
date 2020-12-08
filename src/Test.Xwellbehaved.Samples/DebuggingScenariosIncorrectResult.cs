namespace Xwellbehaved
{
    using Xunit;
    using Xunit.Abstractions;

    public class DebuggingScenariosIncorrectResult : DebuggingScenarios
    {
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

            "When x plus y".x(() => actual = calculator.AssertNotNull().Add(x, y));

            "Then actual equal to expected".x(() => actual.AssertEqual(expected));
        }
    }
}
