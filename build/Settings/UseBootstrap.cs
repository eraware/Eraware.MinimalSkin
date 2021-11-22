namespace Settings
{
    /// <summary>
    /// Defines the possible bootstrap inclusions.
    /// </summary>
    public enum UseBootstrap
    {
        /// <summary>
        /// None of bootstrap.
        /// </summary>
        No,
        /// <summary>
        /// Only loads the responsive utilities and grid system.
        /// </summary>
        ResponsiveUtilitiesOnly,
        /// <summary>
        /// Gets all of bootstrap.
        /// </summary>
        All,
    };
}