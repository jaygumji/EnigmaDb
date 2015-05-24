namespace Enigma.Reflection.Emit
{
    public class ILChainIfCondition
    {
        private readonly ILChainIf _chain;

        public ILChainIfCondition(ILExpressed il, ILGenerationHandler condition)
        {
            _chain = new ILChainIf(il) {
                Condition = condition
            };
        }

        public ILChainIfThen Then(ILGenerationHandler body)
        {
            _chain.Body = body;
            return new ILChainIfThen(_chain);
        }

    }
}