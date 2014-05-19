using System;
using Enigma.Serialization;
using Enigma.Test.Fakes;

namespace Enigma.Test.Serialization
{
    public class CategoryHardCodedTraveller : IGraphTraveller<Category>
    {
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
            visitor.VisitValue(graph.Name, WriteVisitArgs.Value("Name", 1));
            visitor.VisitValue(graph.Description, WriteVisitArgs.Value("Description", 2));
            visitor.VisitValue(graph.Image, WriteVisitArgs.Value("Image", 3));
        }

        public void Travel(IReadVisitor visitor, Category graph)
        {
            String v0;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Name", 1), out v0))
                graph.Name = v0;

            String v1;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Description", 2), out v1))
                graph.Description = v1;

            byte[] v2;
            if (visitor.TryVisitValue(ReadVisitArgs.Value("Image", 3), out v2))
                graph.Image = v2;
        }
    }
}