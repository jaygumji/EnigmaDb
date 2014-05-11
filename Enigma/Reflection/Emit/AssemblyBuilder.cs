using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace Enigma.Reflection.Emit
{
    public class AssemblyBuilder
    {

        private readonly bool _canSave;
        private readonly string _name;
        private readonly System.Reflection.Emit.AssemblyBuilder _assemblyBuilder;
        private readonly ModuleBuilder _module;

        public AssemblyBuilder() : this(false)
        {
        }

        public AssemblyBuilder(bool canSave)
        {
            _canSave = canSave;
            _name = "EnigmaDynamicEmit." + Guid.NewGuid().ToString("N");

            var domain = AppDomain.CurrentDomain;
            _assemblyBuilder = domain.DefineDynamicAssembly(new AssemblyName(_name), canSave ? AssemblyBuilderAccess.RunAndSave : AssemblyBuilderAccess.Run);
            _module = canSave
                ? _assemblyBuilder.DefineDynamicModule(_name, _name + ".dll")
                : _assemblyBuilder.DefineDynamicModule(_name);
        }

        public ClassBuilder DefineClass(string name, Type inherits, Type[] implements)
        {
            const TypeAttributes attributes = TypeAttributes.Class | TypeAttributes.AnsiClass | TypeAttributes.AutoClass | TypeAttributes.BeforeFieldInit;
            return new ClassBuilder(_module.DefineType(_name + name, attributes, inherits, implements));
        }

        public void Save()
        {
            if (!_canSave) throw new InvalidOperationException("Assembly is defined to run only");
            _assemblyBuilder.Save(_name + ".dll");
        }
    }
}