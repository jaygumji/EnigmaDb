using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Enigma.Reflection.Emit
{
    public interface IVariable
    {
        Type VariableType { get; }
    }

    public abstract class Variable : IVariable
    {
        private readonly Type _variableType;

        protected Variable(Type variableType)
        {
            _variableType = variableType;
        }

        public Type VariableType { get { return _variableType; } }
    }

    public class LocalVariable : Variable
    {
        private readonly LocalBuilder _local;

        public LocalVariable(LocalBuilder local) : base(local.LocalType)
        {
            _local = local;
        }

        public LocalBuilder Local { get { return _local; } }

        public static implicit operator LocalVariable(LocalBuilder local)
        {
            return new LocalVariable(local);
        }

    }

    public class MethodArgVariable : Variable
    {
        private readonly int _index;

        public MethodArgVariable(int index, Type variableType) : base(variableType)
        {
            _index = index;
        }

        public int Index
        {
            get { return _index; }
        }

    }

    public class InstancePropertyVariable : Variable
    {
        private readonly IVariable _instance;
        private readonly PropertyInfo _info;

        public InstancePropertyVariable(IVariable instance, PropertyInfo info) : base(info.PropertyType)
        {
            _instance = instance;
            _info = info;
        }

        public IVariable Instance { get { return _instance; } }
        public PropertyInfo Info { get { return _info; } }
    }

}