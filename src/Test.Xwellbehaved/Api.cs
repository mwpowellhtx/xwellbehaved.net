namespace Xwellbehaved
{
    using PublicApiGenerator;
    using Xunit;
    using Xwellbehaved.Infrastructure;

    /// <summary>
    /// For now we are maintaining API testing. We think it does provide some value gauging when
    /// the underlying architecture is likely to change, evaluating whether it should, to what
    /// degree, so on and so forth.
    /// </summary>
    public class Api
    {
        /// <summary>
        /// As a general rule the base line API artifacts should remain untouched. They are
        /// intentionally captured as a reference in order to gauge whether there have been
        /// any appreciable shifts in the classes, methods, etc. As long as we are consistently
        /// meeting our goals with this API, then there should never be a reason to change the
        /// baseline files.
        /// </summary>
        /// <remarks>
        /// This being the case, when we are taking it upon ourselves to intentionally shift
        /// and adjust the API, as we did for tear down methodologies and such, then it becomes
        /// necessary to reconsider the baseline API artifacts, and potentially perform a copy
        /// right into that baseline from the actuals. But only do so under extreme circumstances,
        /// especially when we are seriously evaluating whether API should be updated.
        /// </remarks>
        [Fact]
        public void IsUnchanged() => AssertFile.Contains(

            // TODO: this is just one more area where things can break, in addition to the source code itself...
#if NET8_0
                "../../../api-net8_0.txt"
#endif
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
