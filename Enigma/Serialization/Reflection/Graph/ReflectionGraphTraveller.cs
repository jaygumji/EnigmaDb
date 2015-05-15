namespace Enigma.Serialization.Reflection.Graph
{
    public class ReflectionGraphTraveller<T> : IGraphTraveller<T>
    {
        private readonly IGraphType _type;

        public ReflectionGraphTraveller(IGraphType type)
        {
            _type = type;
        }

        public void Travel(IWriteVisitor visitor, object graph)
        {
            _type.Visit(graph, visitor);
        }

        public void Travel(IReadVisitor visitor, object graph)
        {
            _type.Visit(graph, visitor);
        }

        public void Travel(IWriteVisitor visitor, T graph)
        {
            Travel(visitor, (object)graph);
        }

        public void Travel(IReadVisitor visitor, T graph)
        {
            Travel(visitor, (object)graph);
        }
    }
}
