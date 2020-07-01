namespace Xwellbehaved
{
    using Xunit;

    public class TestFixture
    {
        [Fact]
        public void TestFact()
        {
            Assert.Equal(2, 1 + 1);
        }

        [Scenario]
        public void TestScenario()
        {
            "1 + 1 is 2".x(() => Assert.Equal(2, 1 + 1));
        }
    }
}
