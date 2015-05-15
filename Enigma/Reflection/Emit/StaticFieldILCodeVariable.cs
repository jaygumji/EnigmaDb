using System.Reflection;
using System.Reflection.Emit;

namespace Enigma.Reflection.Emit
{
    public class StaticFieldILCodeVariable : ILCodeVariable
    {
        private readonly FieldInfo _info;

        public StaticFieldILCodeVariable(FieldInfo info)
            : base(info.FieldType)
        {
            _info = info;
        }

        protected override void OnGet(ILExpressed il)
        {
            il.Gen.Emit(OpCodes.Ldsfld, _info);
        }

        protected override void OnGetAddress(ILExpressed il)
        {
            il.Gen.Emit(OpCodes.Ldsflda, _info);
        }

        protected override void OnSet(ILExpressed il)
        {
            il.Gen.Emit(OpCodes.Stsfld, _info);
        }

        public FieldInfo Info { get { return _info; } }
    }
}