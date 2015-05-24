namespace Enigma.Reflection.Emit
{
    public class WhileLoopILCode : IILCode
    {
        private readonly DelegatedILHandler _conditionHandler;
        private readonly DelegatedILHandler _bodyHandler;

        public WhileLoopILCode(ILGenerationMethodHandler conditionHandler, ILGenerationMethodHandler bodyHandler)
        {
            _conditionHandler = new DelegatedILHandler(conditionHandler);
            _bodyHandler = new DelegatedILHandler(bodyHandler);
        }

        public WhileLoopILCode(ILGenerationHandler conditionHandler, ILGenerationHandler bodyHandler)
        {
            _conditionHandler = new DelegatedILHandler(conditionHandler);
            _bodyHandler = new DelegatedILHandler(bodyHandler);
        }

        void IILCode.Generate(ILExpressed il)
        {
            var loopConditionLabel = il.DefineLabel();
            var loopBodyLabel = il.DefineLabel();

            il.TransferLong(loopConditionLabel);
            il.MarkLabel(loopBodyLabel);
            _bodyHandler.Invoke(il);

            il.MarkLabel(loopConditionLabel);
            _conditionHandler.Invoke(il);

            il.TransferLongIfTrue(loopBodyLabel);
        }
    }
}