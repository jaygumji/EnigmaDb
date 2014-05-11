namespace Enigma.Serialization
{
    public class ReadVisitArgs
    {
        public static readonly ReadVisitArgs Item = new ReadVisitArgs("Item", 0, LevelType.Item);
        public static readonly ReadVisitArgs KeyValue = new ReadVisitArgs("KeyValue", 0, LevelType.KeyValue);

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

        public static ReadVisitArgs Root(string name)
        {
            return new ReadVisitArgs(name, 1, LevelType.Root);
        }

    }
}