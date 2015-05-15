namespace Enigma.Reflection.Emit
{
    public class WhileLoopILCode : IILCode
    {
        private readonly ILGenerationHandler _conditionHandler;
        private readonly ILGenerationHandler _bodyHandler;

        public WhileLoopILCode(ILGenerationHandler conditionHandler, ILGenerationHandler bodyHandler)
        {
            _conditionHandler = conditionHandler;
            _bodyHandler = bodyHandler;
        }

        public void Generate(ILExpressed il)
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