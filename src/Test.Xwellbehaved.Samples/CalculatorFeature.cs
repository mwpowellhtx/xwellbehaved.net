namespace Xwellbehaved
{
    using Xunit;
    using Xunit.Abstractions;

    public abstract class CalculatorFeature : SampleFeature
    {
        public CalculatorFeature(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        public class Calculator
        {
            public int Add(int x, int y) => x + y;

            public int Multiply(int x, int y) => x * y;

            public Calculator()
            {
            }
        }
    }
}
