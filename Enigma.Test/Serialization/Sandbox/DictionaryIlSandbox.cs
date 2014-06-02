using System;
using Enigma.Reflection.Emit;
using Enigma.Serialization;

namespace Enigma.Test.Serialization.Sandbox
{
    public class DictionaryIlSandbox
    {
        private AssemblyBuilder _assemblyBuilder;
        private ClassBuilder _classBuilder;
        private MethodBuilder _methodBuilder;

        public DictionaryIlSandbox()
        {
            _assemblyBuilder = new AssemblyBuilder(true);
            _classBuilder = _assemblyBuilder.DefineClass("SandboxClass", typeof(object), Type.EmptyTypes);
            _methodBuilder = _classBuilder.DefineMethod("Execute", typeof(void), new [] {typeof(IReadVisitor), typeof(Fakes.ValueDictionary)});


            _classBuilder.Seal();
        }

        public void Invoke(IReadVisitor visitor, Fakes.ValueDictionary graph)
        {
            var instance = Activator.CreateInstance(typeof (Fakes.ValueDictionary));
        }

    }
}
