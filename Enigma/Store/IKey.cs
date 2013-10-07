namespace Enigma.Store
{
    /// <summary>
    /// A key that identifies an entry in the store
    /// </summary>
    public interface IKey
    {
        /// <summary>
        /// The binary value of the key
        /// </summary>
        byte[] Value { get; }
    }
}
