namespace Xwellbehaved
{
    using Xunit.Abstractions;

    public abstract class SampleFeature
    {
        public ITestOutputHelper OutputHelper { get; }

        protected SampleFeature(ITestOutputHelper outputHelper)
        {
            this.OutputHelper = outputHelper;
        }
    }
}
