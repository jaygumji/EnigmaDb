using System;
using Enigma.Serialization;
using Enigma.Testing.Fakes.Entities;

namespace Enigma.Test.Serialization.HardCoded
{
    public class RelationHardCodedTraveller : IGraphTraveller<Relation>
    {

        private readonly VisitArgs _argsId0;
        private readonly VisitArgs _argsName1;
        private readonly VisitArgs _argsDescription2;
        private readonly VisitArgs _argsValue3;

        public RelationHardCodedTraveller(IVisitArgsFactory factory)
        {
            _argsId0 = factory.Construct("Id");
            _argsName1 = factory.Construct("Name");
            _argsDescription2 = factory.Construct("Description");
            _argsValue3 = factory.Construct("Value");
        }

        public void Travel(IWriteVisitor visitor, Relation graph)
        {
            visitor.VisitValue(graph.Id, _argsId0);
            visitor.VisitValue(graph.Name, _argsName1);
            visitor.VisitValue(graph.Description, _argsDescription2);
            visitor.VisitValue(graph.Value, _argsValue3);
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
            if (visitor.TryVisitValue(_argsId0, out v0) && v0.HasValue)
                graph.Id = v0.Value;

            String v1;
            if (visitor.TryVisitValue(_argsName1, out v1))
                graph.Name = v1;

            String v2;
            if (visitor.TryVisitValue(_argsDescription2, out v2))
                graph.Description = v2;

            Int32? v3;
            if (visitor.TryVisitValue(_argsValue3, out v3) && v3.HasValue)
                graph.Value = v3.Value;
        }
    }
}