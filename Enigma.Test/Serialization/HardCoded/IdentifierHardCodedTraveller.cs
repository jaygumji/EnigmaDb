using Enigma.Serialization;
using Enigma.Testing.Fakes.Entities;

namespace Enigma.Test.Serialization.HardCoded
{
    public class IdentifierHardCodedTraveller : IGraphTraveller<Identifier>
    {
        private readonly VisitArgs _argsId0;
        private readonly VisitArgs _argsType1;

        public IdentifierHardCodedTraveller(IVisitArgsFactory factory)
        {
            _argsId0 = factory.Construct("Id");
            _argsType1 = factory.Construct("Type");
        }

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
            visitor.VisitValue(graph.Id, _argsId0);
            visitor.VisitValue((int)graph.Type, _argsType1);
        }

        public void Travel(IReadVisitor visitor, Identifier graph)
        {
            int? v0;
            if (visitor.TryVisitValue(_argsId0, out v0) && v0.HasValue)
                graph.Id = v0.Value;

            int? v1;
            if (visitor.TryVisitValue(_argsType1, out v1) && v1.HasValue)
                graph.Type = (ApplicationType) v1.Value;
        }
    }
}