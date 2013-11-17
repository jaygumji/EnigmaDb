namespace Enigma.Db.Embedded
{
    /// <summary>
    /// Used to configure engine specific parameters
    /// </summary>
    public class EmbeddedEnigmaEngineConfiguration
    {
        /// <summary>
        /// Gets or sets a value indicating whether [update indexes in background].
        /// </summary>
        /// <value>
        /// <c>true</c> if [update indexes in background]; otherwise, <c>false</c>.
        /// </value>
        public bool UpdateIndexesInBackground { get; set; }
    }
}