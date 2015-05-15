using System.Reflection;

namespace Enigma.Serialization.Reflection
{
    public interface IPropertyMetadataProvider
    {
        uint GetIndexOf(PropertyInfo property);
        bool IsSerializable(PropertyInfo property);
        void SetupArguments(Arguments args);
    }
}