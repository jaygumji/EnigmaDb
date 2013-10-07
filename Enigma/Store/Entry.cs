using Enigma.Store.Binary;

namespace Enigma.Store
{
    /// <summary>
    /// An entry in the store
    /// </summary>
    public class Entry : IEntry
    {

        /// <summary>
        /// Creates a new instance of <see cref="Entry"/>
        /// </summary>
        public Entry()
        {
        }

        /// <summary>
        /// The offset of this entry
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        /// The key that identifies this entry
        /// </summary>
        public IKey Key { get; set; }

        /// <summary>
        /// Offset to the value being stored
        /// </summary>
        public long ValueOffset { get; set; }

        /// <summary>
        /// Length of the value being stored
        /// </summary>
        public long ValueLength { get; set; }

        /// <summary>
        /// Whether the entry is reserved
        /// </summary>
        public bool IsReserved { get; set; }
    }
}
