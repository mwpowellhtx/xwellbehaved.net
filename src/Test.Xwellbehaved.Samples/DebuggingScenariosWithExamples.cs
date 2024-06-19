namespace Xwellbehaved
{
    using Xunit;
    using Xunit.Abstractions;

    public class DebuggingScenariosWithExamples : DebuggingScenarios
    {
        public DebuggingScenariosWithExamples(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

#pragma warning disable xUnit1008 // Test data attribute should only be used on a Theory
        /// <summary>
        /// Do not let the pragma warning fool you. This does work
        /// in order to override the base class Addition Scenario.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="calculator"></param>
        [Example(1, 2, 3)
            , Example(2, 3, 5)]
        public override void Addition(int x, int y, int expected, int actual, Calculator calculator)
        {
            $"Given a number {x}".x(() => { });

            $"Given a number {y}".x(() => { });

            $"Given an expected result {expected}".x(() => { });

            "And given calculator".x(() => calculator = new Calculator());

            // TODO: ditto generic 'AssertNotNull T is class'
            "When x plus y".x(() => actual = calculator.AssertNotNull().AssertIsType<Calculator>().Add(x, y));

            "Then actual equal to expected".x(() => actual.AssertEqual(expected));
        }
    }
}
