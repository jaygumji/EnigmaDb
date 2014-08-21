using System.Reflection.Emit;

namespace Enigma.Reflection.Emit
{
    public class LocalILCodeVariable : ILCodeVariable
    {
        private readonly LocalBuilder _local;

        public LocalILCodeVariable(LocalBuilder local) : base(local.LocalType)
        {
            _local = local;
        }

        public LocalBuilder Local { get { return _local; } }

        protected override void OnGet(ILExpressed il)
        {
            OpCode opCode;
            if (OpCodesLookups.GetLocal.TryGetValue(_local.LocalIndex, out opCode)) {
                il.Gen.Emit(opCode);
                return;
            }
            il.Gen.Emit(OpCodes.Ldloc_S, _local.LocalIndex);
        }

        protected override void OnGetAddress(ILExpressed il)
        {
            il.Gen.Emit(OpCodes.Ldloca_S, _local.LocalIndex);
        }

        protected override void OnSet(ILExpressed il)
        {
            OpCode opCode;
            if (OpCodesLookups.SetLocal.TryGetValue(_local.LocalIndex, out opCode)) {
                il.Gen.Emit(opCode);
                return;
            }

            il.Gen.Emit(OpCodes.Stloc_S, _local.LocalIndex);
        }

        public static implicit operator LocalILCodeVariable(LocalBuilder local)
        {
            return new LocalILCodeVariable(local);
        }

    }
}