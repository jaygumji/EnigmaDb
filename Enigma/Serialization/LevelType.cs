using System;

namespace Enigma.Serialization
{
    [Flags]
    public enum LevelType
    {
        Value = 1,
        Single = 2,
        Root = 4,
        Collection = 8,
        CollectionItem = 16,
        Dictionary = 32,
        DictionaryKey = 64,
        DictionaryValue = 128,

        CollectionInCollection = Collection | CollectionItem,
        DictionaryInCollection = Dictionary | CollectionItem,
        DictionaryInDictionaryKey = Dictionary | DictionaryKey,
        DictionaryInDictionaryValue = Dictionary | DictionaryValue,
        CollectionInDictionaryKey = Collection | DictionaryKey,
        CollectionInDictionaryValue = Collection | DictionaryValue
    }
}