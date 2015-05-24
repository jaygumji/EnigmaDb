namespace Enigma.Reflection.Emit
{

    public class DelegatedILHandler<T> where T : ILCodeParameter
    {
        private readonly ILGenerationMethodHandler<T> _handler;
        private readonly ILGenerationHandler<T> _parameterlessHandler;

        public DelegatedILHandler(ILGenerationMethodHandler<T> handler)
            : this(handler, null)
        {
        }

        public DelegatedILHandler(ILGenerationHandler<T> parameterlessHandler)
            : this(null, parameterlessHandler)
        {
        }

        public DelegatedILHandler(ILGenerationMethodHandler<T> handler, ILGenerationHandler<T> parameterlessHandler)
        {
            _handler = handler;
            _parameterlessHandler = parameterlessHandler;
        }

        public void Invoke(ILExpressed il, T parameter)
        {
            if (_handler != null) _handler.Invoke(il, parameter);
            else if (_parameterlessHandler != null) _parameterlessHandler.Invoke(parameter);
        }
    }

    public class DelegatedILHandler
    {
        private readonly ILGenerationMethodHandler _handler;
        private readonly ILGenerationHandler _parameterlessHandler;

        public DelegatedILHandler(ILGenerationMethodHandler handler) : this(handler, null)
        {
        }

        public DelegatedILHandler(ILGenerationHandler parameterlessHandler) : this(null, parameterlessHandler)
        {
        }

        public DelegatedILHandler(ILGenerationMethodHandler handler, ILGenerationHandler parameterlessHandler)
        {
            _handler = handler;
            _parameterlessHandler = parameterlessHandler;
        }

        public void Invoke(ILExpressed il)
        {
            if (_handler != null) _handler.Invoke(il);
            else if (_parameterlessHandler != null) _parameterlessHandler.Invoke();
        }

    }
}