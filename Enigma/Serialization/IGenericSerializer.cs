using System;
using System.IO;

namespace Enigma.Serialization
{
    public interface IGenericSerializer
    {
        void Serialize(Stream stream, object graph);
        void Serialize<T>(Stream stream, T graph);

        object Deserialize(Stream stream, Type type);
        T Deserialize<T>(Stream stream);
    }
}