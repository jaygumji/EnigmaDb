using System;
using System.Reflection;

namespace Enigma.Reflection.Emit
{
    public class ConstructorBuilder
    {
        private readonly System.Reflection.Emit.ConstructorBuilder _builder;
        private readonly ILExpressed _il;

        public ConstructorBuilder(System.Reflection.Emit.ConstructorBuilder builder)
        {
            _builder = builder;
            _il = new ILExpressed(builder.GetILGenerator());
        }

        public ILExpressed IL { get { return _il; } }
        public ConstructorInfo Reference { get { return _builder; } }

    }
}