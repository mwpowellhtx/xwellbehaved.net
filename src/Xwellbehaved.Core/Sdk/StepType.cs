namespace Xwellbehaved.Sdk
{
    /// <summary>
    /// Indicates the Type of Step being performed.
    /// </summary>
    public enum StepType
    {
        /// <summary>
        /// A Background Step.
        /// </summary>
        Background,

        /// <summary>
        /// A Scenario Step.
        /// </summary>
        Scenario,

        /// <summary>
        /// A TearDown Step.
        /// </summary>
        TearDown,

        /// <summary>
        /// A Rollback Step.
        /// </summary>
        Rollback
    }
}
