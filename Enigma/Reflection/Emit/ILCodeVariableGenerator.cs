using System;
using System.Reflection;

namespace Enigma.Reflection.Emit
{
    public sealed class ILCodeVariableGenerator
    {
        
        private static readonly MethodArgILCodeVariable ThisInstance = new MethodArgILCodeVariable(0, typeof(object));

        private readonly ILExpressed _il;

        public ILCodeVariableGenerator(ILExpressed il)
        {
            _il = il;
        }

        public InstanceFieldILCodeVariable Field(ILCodeVariable instance, FieldInfo field)
        {
            return new InstanceFieldILCodeVariable(instance, field);
        }

        public StaticFieldILCodeVariable Field(FieldInfo field)
        {
            return new StaticFieldILCodeVariable(field);
        }

        public InstancePropertyILCodeVariable Property(ILCodeVariable instance, PropertyInfo property)
        {
            return new InstancePropertyILCodeVariable(instance, property);
        }

        public MethodArgILCodeVariable Arg(int index)
        {
            return Arg(index, typeof (object));
        }

        public MethodArgILCodeVariable Arg(int index, Type argType)
        {
            return new MethodArgILCodeVariable(index, argType);
        }

        public MethodArgILCodeVariable This()
        {
            return ThisInstance;
        }

        public MethodArgILCodeVariable This(Type type)
        {
            return Arg(0, type);
        }

        public void Load(ILCodeVariable variable)
        {
            if (variable == null) throw new ArgumentNullException("variable");
            ((IILCodeVariable)variable).Get(_il);
        }

        public void LoadAddress(ILCodeVariable variable)
        {
            if (variable == null) throw new ArgumentNullException("variable");
            ((IILCodeVariable)variable).GetAddress(_il);
        }

        public void Set(ILCodeVariable variable)
        {
            if (variable == null) throw new ArgumentNullException("variable");
            ((IILCodeVariable)variable).Set(_il);
        }

    }
}