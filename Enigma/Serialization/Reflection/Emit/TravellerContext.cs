using System;
using System.Collections.Generic;
using System.Reflection;

namespace Enigma.Serialization.Reflection.Emit
{
    public class TravellerContext
    {

        private readonly IReadOnlyDictionary<Type, ChildTravellerInfo> _childTravellers;
        private readonly IReadOnlyDictionary<SerializableProperty, FieldInfo> _argFields;

        public TravellerContext(IReadOnlyDictionary<Type, ChildTravellerInfo> childTravellers, IReadOnlyDictionary<SerializableProperty, FieldInfo> argFields)
        {
            _childTravellers = childTravellers;
            _argFields = argFields;
        }

        public ChildTravellerInfo GetTraveller(Type type)
        {
            ChildTravellerInfo childTravellerInfo;
            if (!_childTravellers.TryGetValue(type, out childTravellerInfo))
                throw InvalidGraphException.ComplexTypeWithoutTravellerDefined(type);

            return childTravellerInfo;
        }

        public FieldInfo GetArgsField(SerializableProperty ser)
        {
            FieldInfo field;
            if (!_argFields.TryGetValue(ser, out field))
                throw new ArgumentException("Could not find the visit args field for the property " + ser.Ref.Name);

            return field;
        }
    }
}