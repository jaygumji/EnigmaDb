using System.Reflection;

namespace Enigma.Reflection.Emit
{
    public class InstancePropertyILCodeVariable : ILCodeVariable
    {
        private readonly ILCodeVariable _instance;
        private readonly PropertyInfo _info;

        public InstancePropertyILCodeVariable(ILCodeVariable instance, PropertyInfo info) : base(info.PropertyType)
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

            if (_instance.VariableType.IsValueType)
                il.Call(_info.GetGetMethod());
            else
                il.CallVirt(_info.GetGetMethod());
        }

        protected override void OnGetAddress(ILExpressed il)
        {
            OnGetAddress(il);
            var local = il.DeclareLocal("addressOf" + _info.Name, _info.PropertyType);
            il.Var.Set(local);
            il.Var.LoadAddress(local);
        }

        protected override void OnSet(ILExpressed il)
        {
            if (_instance.VariableType.IsValueType)
                il.Var.LoadAddress(_instance);
            else
                il.Var.Load(_instance);

            if (VariableType.IsValueType)
                il.Call(_info.GetSetMethod());
            else
                il.CallVirt(_info.GetSetMethod());
        }

        public ILCodeVariable Instance { get { return _instance; } }
        public PropertyInfo Info { get { return _info; } }
    }
}