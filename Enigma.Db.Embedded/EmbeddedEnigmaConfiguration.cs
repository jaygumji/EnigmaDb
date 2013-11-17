namespace Enigma.Db.Embedded
{
    /// <summary>
    /// Used to configure an embedded enigma environment
    /// </summary>
    public class EmbeddedEnigmaConfiguration
    {
        private readonly EmbeddedEnigmaEngineConfiguration _engine;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedEnigmaConfiguration"/> class.
        /// </summary>
        public EmbeddedEnigmaConfiguration()
        {
            _engine = new EmbeddedEnigmaEngineConfiguration();
        }

        /// <summary>
        /// Gets the engine configuration.
        /// </summary>
        /// <value>
        /// The engine configuration.
        /// </value>
        public EmbeddedEnigmaEngineConfiguration Engine { get { return _engine; } }

    }
}
