using System.IO;
using Enigma.IO;

namespace Enigma.Serialization.PackedBinary
{
    public class PackedDataSerializer<T> : ITypedSerializer<T>
    {
        private readonly SerializationEngine _engine;

        public PackedDataSerializer()
        {
            _engine = new SerializationEngine();
        }

        void ITypedSerializer.Serialize(Stream stream, object graph)
        {
            Serialize(stream, (T) graph);
        }

        public T Deserialize(Stream stream)
        {
            var visitor = new PackedDataReadVisitor(stream);
            return _engine.Deserialize<T>(visitor);
        }

        public void Serialize(Stream stream, T graph)
        {
            var visitor = new PackedDataWriteVisitor(stream);
            _engine.Serialize(visitor, graph);
        }

        object ITypedSerializer.Deserialize(Stream stream)
        {
            return Deserialize(stream);
        }
    }
}
