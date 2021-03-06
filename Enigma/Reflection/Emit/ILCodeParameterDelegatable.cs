using System;

namespace Enigma.Reflection.Emit
{
    public class ILCodeParameterDelegatable : ILCodeParameter
    {
        private readonly Type _parameterType;
        private readonly ILGenerationMethodHandler _valueLoader;
        private readonly ILGenerationMethodHandler _valueAddressLoader;

        public ILCodeParameterDelegatable(Type parameterType, ILGenerationMethodHandler valueLoader) : this(parameterType, valueLoader, null)
        {
        }

        public ILCodeParameterDelegatable(Type parameterType, ILGenerationMethodHandler valueLoader, ILGenerationMethodHandler valueAddressLoader)
        {
            _parameterType = parameterType;
            _valueLoader = valueLoader;
            _valueAddressLoader = valueAddressLoader;
        }

        public override Type ParameterType
        {
            get { return _parameterType; }
        }

        protected override void Load(ILExpressed il)
        {
            _valueLoader.Invoke(il);
        }

        protected override void LoadAddress(ILExpressed il)
        {
            if (_valueAddressLoader == null)
                throw new NotSupportedException("This parameter does not support address loading");

            _valueAddressLoader.Invoke(il);
        }
    }
}