namespace Enigma.Reflection.Emit
{
    public class ILChainIfElse
    {
        private readonly ILChainIf _chain;

        public ILChainIfElse(ILChainIf chain)
        {
            _chain = chain;
        }

        public void End()
        {
            _chain.End();
        }

    }
}