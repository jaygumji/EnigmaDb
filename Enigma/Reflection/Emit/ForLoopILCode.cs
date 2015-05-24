using System;
using System.Runtime.InteropServices;

namespace Enigma.Reflection.Emit
{

    public class ForLoopILCode : IILCode
    {
        private readonly ILCodeParameter _initialValue;
        private readonly ILCodeParameter _increment;

        private readonly DelegatedILHandler<ILCodeParameter> _conditionHandler;
        private readonly DelegatedILHandler<ILCodeParameter> _bodyHandler;

        public ForLoopILCode(ILCodeParameter initialValue, ILGenerationMethodHandler<ILCodeParameter> conditionHandler,
            ILGenerationMethodHandler<ILCodeParameter> bodyHandler, ILCodeParameter increment)
        {
            if (initialValue.ParameterType != increment.ParameterType)
                throw new ArgumentException("The type of the initial value and the increment value must match");

            _initialValue = initialValue;
            _increment = increment;
            _conditionHandler = new DelegatedILHandler<ILCodeParameter>(conditionHandler);
            _bodyHandler = new DelegatedILHandler<ILCodeParameter>(bodyHandler);
        }

        public ForLoopILCode(ILCodeParameter initialValue, ILGenerationHandler<ILCodeParameter> conditionHandler,
            ILGenerationHandler<ILCodeParameter> bodyHandler, ILCodeParameter increment)
        {
            if (initialValue.ParameterType != increment.ParameterType)
                throw new ArgumentException("The type of the initial value and the increment value must match");

            _initialValue = initialValue;
            _increment = increment;
            _conditionHandler = new DelegatedILHandler<ILCodeParameter>(conditionHandler);
            _bodyHandler = new DelegatedILHandler<ILCodeParameter>(bodyHandler);
        }

        void IILCode.Generate(ILExpressed il)
        {
            var value = il.DeclareLocal("index", _initialValue.ParameterType);
            il.Snippets.SetVariable(value, _initialValue);

            var labelCondition = il.DefineLabel();
            il.TransferLong(labelCondition);

            var labelBody = il.DefineAndMarkLabel();
            _bodyHandler.Invoke(il, value);
            il.Snippets.Increment(value, _increment);

            il.MarkLabel(labelCondition);
            _conditionHandler.Invoke(il, value);

            il.TransferLongIfTrue(labelBody);
        }
    }

}