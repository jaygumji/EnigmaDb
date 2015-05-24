using System;

namespace Enigma.Reflection.Emit
{
    public class CastILCodeParameter : ILCodeParameter
    {
        private readonly IILCodeParameter _parameter;
        private readonly Type _toType;

        public CastILCodeParameter(IILCodeParameter parameter, Type toType)
        {
            _parameter = parameter;
            _toType = toType;
        }

        public override Type ParameterType
        {
            get { return _toType; }
        }

        protected override void Load(ILExpressed il)
        {
            _parameter.Load(il);
            if (_parameter.ParameterType != _toType)
                il.Cast(_toType);
        }

    }
}