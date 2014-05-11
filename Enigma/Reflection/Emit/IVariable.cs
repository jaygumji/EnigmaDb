using System;
using System.Reflection.Emit;

namespace Enigma.Reflection.Emit
{
    public interface IVariable
    {
        Type VariableType { get; }
    }

    public class LocalVariable : IVariable
    {
        private readonly LocalBuilder _local;

        public LocalVariable(LocalBuilder local)
        {
            _local = local;
        }

        public LocalBuilder Local { get { return _local; } }
        public Type VariableType { get { return _local.LocalType; } }
    }

    public class MethodArgVariable : IVariable
    {
        private readonly int _index;
        private readonly Type _variableType;

        public MethodArgVariable(int index, Type variableType)
        {
            _index = index;
            _variableType = variableType;
        }

        public int Index
        {
            get { return _index; }
        }

        public Type VariableType
        {
            get { return _variableType; }
        }
    }
}