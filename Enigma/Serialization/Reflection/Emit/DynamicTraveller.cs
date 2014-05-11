using System;
using System.Linq;
using System.Reflection;

namespace Enigma.Serialization.Reflection.Emit
{
    public class DynamicTraveller
    {
        private static readonly object[] EmptyParameters = {};

        private Type _travellerType;
        private ConstructorInfo _constructor;
        private MethodInfo _travelWriteMethod;
        private MethodInfo _travelReadMethod;
        private bool _isConstructing;

        public DynamicTraveller(Type travellerType, ConstructorInfo constructor, MethodInfo travelWriteMethod, MethodInfo travelReadMethod)
        {
            _travellerType = travellerType;
            _constructor = constructor;
            _travelWriteMethod = travelWriteMethod;
            _travelReadMethod = travelReadMethod;
            _isConstructing = true;
        }

        public Type TravellerType { get { return _travellerType; } }
        public ConstructorInfo Constructor { get { return _constructor; } }
        public MethodInfo TravelWriteMethod { get { return _travelWriteMethod; } }
        public MethodInfo TravelReadMethod { get { return _travelReadMethod; } }

        public void Complete(Type actualTravellerType)
        {
            _travellerType = actualTravellerType;
            _constructor = actualTravellerType.GetConstructor(Type.EmptyTypes);
            _travelWriteMethod = actualTravellerType.GetMethod("Travel", _travelWriteMethod.GetParameters().Select(p => p.ParameterType).ToArray());
            _travelReadMethod = actualTravellerType.GetMethod("Travel", _travelReadMethod.GetParameters().Select(p => p.ParameterType).ToArray());
            _isConstructing = false;
        }

        public IGraphTraveller GetInstance()
        {
            if (_isConstructing) throw new InvalidOperationException("The type is still being constructed");
            return (IGraphTraveller) _constructor.Invoke(EmptyParameters);
        }
    }
}