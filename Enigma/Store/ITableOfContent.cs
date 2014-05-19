using System.Collections.Generic;

namespace Enigma.Store
{
    /// <summary>
    /// A table of content for a store
    /// </summary>
    public interface ITableOfContent
    {
        /// <summary>
        /// Total number of entries currently in the store
        /// </summary>
        int Count { get; }

        /// <summary>
        /// All the entries in this table of content
        /// </summary>
        IEnumerable<Entry> Entries { get; }

        /// <summary>
        /// Try get an entry
        /// </summary>
        /// <param name="key">The key that identifies the entry</param>
        /// <param name="entry">The entry that was found</param>
        /// <returns>true if an entry was found, otherwise false</returns>
        bool TryGet(IKey key, out IEntry entry);

        /// <summary>
        /// Find out whether this table of content contains an entry
        /// </summary>
        /// <param name="key">DictionaryKey</param>
        /// <returns>true if the table of content contains an entry with the matching key, otherwise false</returns>
        bool Contains(IKey key);
    }
}
