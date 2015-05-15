namespace Enigma.Serialization.Reflection
{
    public class SerializationMetadata
    {
        public static readonly SerializationMetadata Root = new SerializationMetadata(1, new Arguments());
        public static readonly SerializationMetadata Item = new SerializationMetadata(0, new Arguments());

        private readonly uint _index;
        private readonly Arguments _args;

        public SerializationMetadata(uint index, Arguments args)
        {
            _index = index;
            _args = args;
        }

        public uint Index { get { return _index; } }
        public Arguments Args { get { return _args; } }
    }
}