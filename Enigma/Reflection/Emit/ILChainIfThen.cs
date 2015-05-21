namespace Enigma.Reflection.Emit
{
    public class ILChainIfThen
    {
        private readonly ILChainIf _chain;

        public ILChainIfThen(ILChainIf chain)
        {
            _chain = chain;
        }

        public ILChainIfElse Else(ILGenerationChainHandler body)
        {
            _chain.ElseBody = body;
            return new ILChainIfElse(_chain);
        }

        public void End()
        {
            _chain.End();
        }

    }
}