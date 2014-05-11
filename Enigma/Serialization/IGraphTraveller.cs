namespace Enigma.Serialization
{
    public interface IGraphTraveller
    {
        void Travel(IWriteVisitor visitor, object graph);
        void Travel(IReadVisitor visitor, object graph);
    }

    public interface IGraphTraveller<in T> : IGraphTraveller
    {
        void Travel(IWriteVisitor visitor, T graph);
        void Travel(IReadVisitor visitor, T graph);
    }
}
