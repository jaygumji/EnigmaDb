namespace Enigma.Serialization.Reflection.Graph
{
    public interface IGraphProperty
    {
        void Visit(object graph, IReadVisitor visitor);
        void Visit(object graph, IWriteVisitor visitor);
    }
}