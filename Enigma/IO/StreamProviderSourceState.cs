namespace Enigma.IO
{
    /// <summary>
    /// The provider source state
    /// </summary>
    public enum StreamProviderSourceState
    {
        /// <summary>
        /// The provider created the source
        /// </summary>
        Created,

        /// <summary>
        /// The provider reconnected to the source
        /// </summary>
        Reconnected
    }
}