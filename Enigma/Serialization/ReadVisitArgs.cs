namespace Enigma.Serialization
{
    public class ReadVisitArgs
    {
        public static readonly ReadVisitArgs CollectionItem = new ReadVisitArgs("CollectionItem", 0, LevelType.CollectionItem);
        public static readonly ReadVisitArgs DictionaryKey = new ReadVisitArgs("DictionaryKey", 0, LevelType.DictionaryKey);
        public static readonly ReadVisitArgs DictionaryValue = new ReadVisitArgs("DictionaryValue", 0, LevelType.DictionaryValue);

        private readonly string _name;
        private readonly uint _index;
        private readonly LevelType _type;
        public object State;

        public ReadVisitArgs(string name, uint index, LevelType type)
        {
            _name = name;
            _index = index;
            _type = type;
        }

        public string Name
        {
            get { return _name; }
        }

        public uint Index
        {
            get { return _index; }
        }

        public LevelType Type
        {
            get { return _type; }
        }

        public static ReadVisitArgs Value(string name, uint index)
        {
            return new ReadVisitArgs(name, index, LevelType.Value);
        }

        public static ReadVisitArgs NullableValue(string name, uint index)
        {
            return new ReadVisitArgs(name, index, LevelType.Value);
        }

        public static ReadVisitArgs Single(string name, uint index)
        {
            return new ReadVisitArgs(name, index, LevelType.Single);
        }

        public static ReadVisitArgs Collection(string name, uint index)
        {
            return new ReadVisitArgs(name, index, LevelType.Collection);
        }

        public static ReadVisitArgs Dictionary(string name, uint index)
        {
            return new ReadVisitArgs(name, index, LevelType.Dictionary);
        }

        public static ReadVisitArgs Root(string name)
        {
            return new ReadVisitArgs(name, 1, LevelType.Root);
        }

    }
}