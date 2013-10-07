namespace Enigma.Store
{
    /// <summary>
    /// An entry in the store
    /// </summary>
    public interface IEntry
    {
        /// <summary>
        /// The key that identifies this entry
        /// </summary>
        IKey Key { get; }

        /// <summary>
        /// Offset to the value being stored
        /// </summary>
        long ValueOffset { get; }

        /// <summary>
        /// Length of the value being stored
        /// </summary>
        long ValueLength { get; }
    }
}
