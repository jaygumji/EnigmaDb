using System;
using System.Reflection.Emit;

namespace Enigma.Reflection.Emit
{
    public class ILChainIf
    {
        private readonly ILExpressed _il;

        public ILChainIf(ILExpressed il)
        {
            _il = il;
        }

        public ILGenerationHandler Condition { get; set; }
        public ILGenerationHandler Body { get; set; }
        public ILGenerationHandler ElseBody { get; set; }

        public void End()
        {
            if (Condition == null) throw new InvalidOperationException("No condition set");
            if (Body == null) throw new InvalidOperationException("No body set");

            Condition.Invoke();

            var endLabel = _il.DefineLabel();

            var elseLabel = default(Label);
            if (ElseBody != null) {
                elseLabel = _il.DefineLabel();
                _il.TransferLongIfFalse(elseLabel);
            }
            else {
                _il.TransferLongIfFalse(endLabel);
            }

            Body.Invoke();

            if (ElseBody != null) {
                _il.TransferLong(endLabel);

                _il.MarkLabel(elseLabel);
                ElseBody.Invoke();
            }
            
            _il.MarkLabel(endLabel);
        }
    }
}