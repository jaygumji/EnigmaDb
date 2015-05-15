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
            foreach (var parameter in _parameters.Cast<IILCodeParameter>())
                parameter.Load(il);

            il.Gen.Emit(OpCodes.Newobj, _constructor);
        }
    }
}