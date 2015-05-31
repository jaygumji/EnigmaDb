using System.Collections.Generic;
using Enigma.Serialization;
using Enigma.Testing.Fakes.Graphs;

namespace Enigma.Test.Serialization.HardCoded
{
    public class MultidimensionalArrayGraphHardCodedTraveller : IGraphTraveller<MultidimensionalArrayGraph>
    {
        private readonly VisitArgs _argsValue;

        public MultidimensionalArrayGraphHardCodedTraveller(IVisitArgsFactory factory)
        {
            _argsValue = factory.Construct("Value");
        }

        public void Travel(IWriteVisitor visitor, object graph)
        {
            Travel(visitor, (MultidimensionalArrayGraph) graph);
        }

        public void Travel(IReadVisitor visitor, object graph)
        {
            Travel(visitor, (MultidimensionalArrayGraph) graph);
        }

        public void Travel(IWriteVisitor visitor, MultidimensionalArrayGraph graph)
        {
            {
                var c = graph.Value;
                visitor.Visit(c, _argsValue);

                if (c != null) {
                    for (var r0 = 0; r0 < c.GetLength(0); r0++) {
                        visitor.Visit(c, VisitArgs.CollectionInCollection);
                        for (var r1 = 0; r1 < c.GetLength(1); r1++) {
                            visitor.VisitValue(c[r0, r1], VisitArgs.CollectionItem);
                        }
                        visitor.Leave(c, VisitArgs.CollectionInCollection);
                    }
                }

                visitor.Leave(c, _argsValue);
            }
        }

        public void Travel(IReadVisitor visitor, MultidimensionalArrayGraph graph)
        {
            ValueState state;
            state = visitor.TryVisit(_argsValue);
            if (state != ValueState.NotFound) {
                if (state == ValueState.Found) {
                    ICollection<ICollection<int>> c = new List<ICollection<int>>();
                    while (visitor.TryVisit(VisitArgs.CollectionInCollection) == ValueState.Found) {
                        ICollection<int> cv = new List<int>();

                        int? cvv;
                        while (visitor.TryVisitValue(VisitArgs.CollectionItem, out cvv) && cvv.HasValue)
                            cv.Add(cvv.Value);

                        visitor.Leave(VisitArgs.CollectionInCollection);
                        c.Add(cv);
                    }
                    graph.Value = ArrayProvider.To2DArray(c);

                    visitor.Leave(_argsValue);
                }
                else
                    graph.Value = null;
            }
        }
    }
}