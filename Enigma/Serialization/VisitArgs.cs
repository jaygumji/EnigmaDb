using Enigma.Serialization.Reflection;

namespace Enigma.Serialization
{
    public class VisitArgs
    {
        public static readonly VisitArgs CollectionItem = new VisitArgs("CollectionItem", SerializationMetadata.Item, LevelType.CollectionItem);
        public static readonly VisitArgs DictionaryKey = new VisitArgs("DictionaryKey", SerializationMetadata.Item, LevelType.DictionaryKey);
        public static readonly VisitArgs DictionaryValue = new VisitArgs("DictionaryValue", SerializationMetadata.Item, LevelType.DictionaryValue);
        public static readonly VisitArgs CollectionInCollection = new VisitArgs("Collection in Collection", SerializationMetadata.Item, LevelType.CollectionInCollection);
        public static readonly VisitArgs DictionaryInCollection = new VisitArgs("Dictionary in Collection", SerializationMetadata.Item, LevelType.DictionaryInCollection);
        public static readonly VisitArgs DictionaryInDictionaryKey = new VisitArgs("Dictionary in Dictionary Key", SerializationMetadata.Item, LevelType.DictionaryInDictionaryKey);
        public static readonly VisitArgs DictionaryInDictionaryValue = new VisitArgs("Dictionary in Dictionary Value", SerializationMetadata.Item, LevelType.DictionaryInDictionaryValue);
        public static readonly VisitArgs CollectionInDictionaryKey = new VisitArgs("Collection in Dictionary Key", SerializationMetadata.Item, LevelType.CollectionInDictionaryKey);
        public static readonly VisitArgs CollectionInDictionaryValue = new VisitArgs("Collection in Dictionary Value", SerializationMetadata.Item, LevelType.CollectionInDictionaryValue);

        private readonly string _name;
        private readonly SerializationMetadata _metadata;
        private readonly LevelType _type;

        public VisitArgs(string name, SerializationMetadata metadata, LevelType type)
        {
            _name = name;
            _metadata = metadata;
            _type = type;
        }

        public string Name
        {
            get { return _name; }
        }

        public LevelType Type
        {
            get { return _type; }
        }

        public SerializationMetadata Metadata
        {
            get { return _metadata; }
        }

        public override string ToString()
        {
            return string.Concat(Type, " args ", Name, " with index ", Metadata.Index);
        }

        public static VisitArgs Value(string name, SerializationMetadata metadata)
        {
            return new VisitArgs(name, metadata, LevelType.Value);
        }

        public static VisitArgs Single(string name, SerializationMetadata metadata)
        {
            return new VisitArgs(name, metadata, LevelType.Single);
        }

        public static VisitArgs Collection(string name, SerializationMetadata metadata)
        {
            return new VisitArgs(name, metadata, LevelType.Collection);
        }

        public static VisitArgs Dictionary(string name, SerializationMetadata metadata)
        {
            return new VisitArgs(name, metadata, LevelType.Dictionary);
        }

        public static VisitArgs Root(string name)
        {
            return new VisitArgs(name, SerializationMetadata.Root, LevelType.Root);
        }

    }
}