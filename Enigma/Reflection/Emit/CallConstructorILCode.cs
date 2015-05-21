using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Enigma.Reflection.Emit
{
    public class CallConstructorILCode : IILCode
    {
        private readonly ILCodeParameter[] _parameters;
        private readonly ConstructorInfo _constructor;

        public CallConstructorILCode(ConstructorInfo constructor, params ILCodeParameter[] parameters)
        {
            _constructor = constructor;
            _parameters = parameters;
        }

        void IILCode.Generate(ILExpressed il)
        {
            for (var i = 0; i < _parameters.Length; i++) {
                var parameter = (IILCodeParameter) _parameters[i] ?? ILCodeParameter.Null;
                parameter.Load(il);
            }

            il.Gen.Emit(OpCodes.Newobj, _constructor);
        }
    }
}