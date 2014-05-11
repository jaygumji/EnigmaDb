using System;
using Enigma.Serialization;
using Enigma.Test.Fakes;

namespace Enigma.Test.Serialization
{
    public class RelationHardCodedTraveller : IGraphTraveller<Relation>
    {
        public void Travel(IWriteVisitor visitor, Relation graph)
        {
            visitor.VisitValue(graph.Id, WriteVisitArgs.Value("Id", 1));
            visitor.VisitValue(graph.Name, WriteVisitArgs.Value("Name", 2));
            visitor.VisitValue(graph.Description, WriteVisitArgs.Value("Description", 3));
            visitor.VisitValue(graph.Value, WriteVisitArgs.Value("Value", 4));
        }

        public void Travel(IWriteVisitor visitor, object graph)
        {
            Travel(visitor, (Relation) graph);
        }

        public void Travel(IReadVisitor visitor, object graph)
        {
            Travel(visitor, (Relation) graph);
        }

        public void Travel(IReadVisitor visitor, Relation graph)
        {
            Guid? v0;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Id", 1), out v0) && v0.HasValue)
                graph.Id = v0.Value;

            String v1;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Name", 2), out v1))
                graph.Name = v1;

            String v2;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Description", 3), out v2))
                graph.Description = v2;

            Int32? v3;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Value", 4), out v3) && v3.HasValue)
                graph.Value = v3.Value;
        }
    }
}