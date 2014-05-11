using System.Collections;
using System.Collections.Generic;

namespace Enigma.Serialization
{
    public class WriteVisitArgs
    {
        public static readonly WriteVisitArgs Item = new WriteVisitArgs("Item", 0, LevelType.Item);
        public static readonly WriteVisitArgs KeyValue = new WriteVisitArgs("KeyValue", 0, LevelType.KeyValue);

        private readonly string _name;
        private readonly uint _index;
        private readonly LevelType _type;
        private readonly int _count;
        private readonly bool _hasValue;
        public object State;

        public WriteVisitArgs(string name, uint index, LevelType type)
        {
            _name = name;
            _index = index;
            _type = type;
            _count = -1;
            _hasValue = false;
        }

        public WriteVisitArgs(string name, uint index, LevelType type, bool hasValue)
        {
            _name = name;
            _index = index;
            _type = type;
            _count = -1;
            _hasValue = hasValue;
        }

        public WriteVisitArgs(string name, uint index, LevelType type, bool hasValue, int count)
        {
            _name = name;
            _index = index;
            _type = type;
            _count = count;
            _hasValue = hasValue;
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

        public int Count
        {
            get { return _count; }
        }

        public bool HasValue
        {
            get { return _hasValue; }
        }

        public static WriteVisitArgs Value(string name, uint index)
        {
            return new WriteVisitArgs(name, index, LevelType.Value);
        }

        public static WriteVisitArgs NullableValue(string name, uint index, bool hasValue)
        {
            return new WriteVisitArgs(name, index, LevelType.Value, hasValue);
        }

        public static WriteVisitArgs Single(string name, uint index, object level)
        {
            return new WriteVisitArgs(name, index, LevelType.Single, level != null);
        }

        public static WriteVisitArgs Collection<T>(string name, uint index, ICollection<T> collection)
        {
            return new WriteVisitArgs(name, index, LevelType.Collection, collection != null, collection != null ? collection.Count : 0);
        }

        public static WriteVisitArgs Root(string name, object level)
        {
            return new WriteVisitArgs(name, 1, LevelType.Root, level != null);
        }

    }
}