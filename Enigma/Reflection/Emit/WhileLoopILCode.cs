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
            _conditionHandler.Invoke(il);

            var endLabel = il.DefineLabel();
            il.TransferLongIfTrue(endLabel);

        }
    }
}