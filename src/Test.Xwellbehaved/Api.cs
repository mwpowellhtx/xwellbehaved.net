namespace Xwellbehaved
{
    using PublicApiGenerator;
    using Xunit;
    using Xwellbehaved.Infrastructure;

    // TODO: TBD: ditto Xml comments...
    // TODO: TBD: also, is Api generator really that necessary? not least of all in the targets?
    public class Api
    {
        [Fact]
        public void IsUnchanged() => AssertFile.Contains(

            // TODO: TBD: this is just one more area where things can break, in addition to the source code itself...
#if NETCOREAPP3_1
                "../../../api-netcoreapp3_1.txt"
#endif
#if NET472
                "../../../api-net472.txt"
#endif

                , typeof(ScenarioAttribute).Assembly.GeneratePublicApi()
        );
    }
}
