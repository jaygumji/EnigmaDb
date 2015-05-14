using System;
using Enigma.Serialization;
using Enigma.Test.Fakes;

namespace Enigma.Test.Serialization
{
    public class CategoryHardCodedTraveller : IGraphTraveller<Category>
    {
        private readonly VisitArgs _argsName0;
        private readonly VisitArgs _argsDescription1;
        private readonly VisitArgs _argsImage2;

        public CategoryHardCodedTraveller(IVisitArgsFactory factory)
        {
            _argsName0 = factory.Construct("Name");
            _argsDescription1 = factory.Construct("Description");
            _argsImage2 = factory.Construct("Image");
        }

        public void Travel(IWriteVisitor visitor, object graph)
        {
            Travel(visitor, (Category) graph);
        }

        public void Travel(IReadVisitor visitor, object graph)
        {
            Travel(visitor, (Category)graph);
        }

        public void Travel(IWriteVisitor visitor, Category graph)
        {
            visitor.VisitValue(graph.Name, _argsName0);
            visitor.VisitValue(graph.Description, _argsDescription1);
            visitor.VisitValue(graph.Image, _argsImage2);
        }

        public void Travel(IReadVisitor visitor, Category graph)
        {
            String v0;
            if (visitor.TryVisitValue(_argsName0, out v0))
                graph.Name = v0;

            String v1;
            if (visitor.TryVisitValue(_argsDescription1, out v1))
                graph.Description = v1;

            byte[] v2;
            if (visitor.TryVisitValue(_argsImage2, out v2))
                graph.Image = v2;
        }
    }
}