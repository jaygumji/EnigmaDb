using System.Reflection;

namespace Enigma.Reflection.Emit
{
    public class MethodBuilder
    {

        private readonly System.Reflection.Emit.MethodBuilder _methodBuilder;
        private readonly ILExpressed _il;

        public MethodBuilder(System.Reflection.Emit.MethodBuilder methodBuilder)
        {
            _methodBuilder = methodBuilder;
            _il = new ILExpressed(_methodBuilder.GetILGenerator());
        }

        public MethodInfo Method { get { return _methodBuilder; } }
        public ILExpressed IL { get { return _il; } }
    }
}