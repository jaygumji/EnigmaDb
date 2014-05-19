using Enigma.Serialization;
using Enigma.Test.Fakes;

namespace Enigma.Test.Serialization
{
    public class IdentifierHardCodedTraveller : IGraphTraveller<Identifier>
    {
        public void Travel(IWriteVisitor visitor, object graph)
        {
            Travel(visitor, (Identifier) graph);
        }

        public void Travel(IReadVisitor visitor, object graph)
        {
            Travel(visitor, (Identifier) graph);
        }

        public void Travel(IWriteVisitor visitor, Identifier graph)
        {
            visitor.VisitValue(graph.Id, WriteVisitArgs.Value("Id", 1));
            visitor.VisitValue((int)graph.Type, WriteVisitArgs.EnumValue("Type", 2, graph.Type));
        }

        public void Travel(IReadVisitor visitor, Identifier graph)
        {
            int? v0;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Id", 1), out v0) && v0.HasValue)
                graph.Id = v0.Value;

            int? v1;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Type", 2), out v1) && v1.HasValue)
                graph.Type = (ApplicationType) v1.Value;
        }
    }
}