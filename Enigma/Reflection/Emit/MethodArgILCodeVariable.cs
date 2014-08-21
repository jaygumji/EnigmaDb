using System;
using System.Reflection.Emit;

namespace Enigma.Reflection.Emit
{
    public class MethodArgILCodeVariable : ILCodeVariable
    {
        private readonly int _index;

        public MethodArgILCodeVariable(int index, Type variableType) : base(variableType)
        {
            _index = index;
        }

        public override bool CanSet
        {
            get { return false; }
        }

        protected override void OnGet(ILExpressed il)
        {
            OpCode opCode;
            if (OpCodesLookups.LoadArg.TryGetValue(_index, out opCode))
                il.Gen.Emit(opCode);
            else
                il.Gen.Emit(OpCodes.Ldarg_S, _index);
        }

        protected override void OnGetAddress(ILExpressed il)
        {
            il.Gen.Emit(OpCodes.Ldarga_S, _index);
        }

        public int Index
        {
            get { return _index; }
        }

    }
}