namespace Enigma.Serialization.Reflection.Graph
{
    public interface IGraphType
    {
        void Visit(object graph, IReadVisitor visitor);
        void Visit(object graph, IWriteVisitor visitor);
    }
}