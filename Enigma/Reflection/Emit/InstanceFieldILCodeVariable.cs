using System.Reflection;
using System.Reflection.Emit;

namespace Enigma.Reflection.Emit
{
    public class InstanceFieldILCodeVariable : ILCodeVariable
    {
        private readonly ILCodeVariable _instance;
        private readonly FieldInfo _info;

        public InstanceFieldILCodeVariable(ILCodeVariable instance, FieldInfo info) : base(info.FieldType)
        {
            _instance = instance;
            _info = info;
        }

        protected override void OnGet(ILExpressed il)
        {
            if (_instance.VariableType.IsValueType)
                il.Var.LoadAddress(_instance);
            else
                il.Var.Load(_instance);

            il.Gen.Emit(OpCodes.Ldfld, _info);
        }

        protected override void OnGetAddress(ILExpressed il)
        {
            if (_instance.VariableType.IsValueType)
                il.Var.LoadAddress(_instance);
            else
                il.Var.Load(_instance);

            il.Gen.Emit(OpCodes.Ldflda, _info);
        }

        protected override void OnSet(ILExpressed il)
        {
            if (_instance.VariableType.IsValueType)
                il.Var.LoadAddress(_instance);
            else
                il.Var.Load(_instance);

            il.Gen.Emit(OpCodes.Stfld, _info);
        }

        public ILCodeVariable Instance { get { return _instance; } }
        public FieldInfo Info { get { return _info; } }
    }
}