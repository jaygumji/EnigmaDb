using System.Reflection;

namespace Enigma.Serialization.Reflection.Emit
{
    public class ChildTravellerInfo
    {
        public FieldInfo Field;
        public MethodInfo TravelWriteMethod;
        public MethodInfo TravelReadMethod;
    }
}