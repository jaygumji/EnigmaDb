using Enigma.Serialization;
using Enigma.Test.Serialization.Fakes;

namespace Enigma.Test.Serialization
{
    public class NullableValuesEntityHardCodedTraveller : IGraphTraveller<NullableValuesEntity>
    {
        public void Travel(IWriteVisitor visitor, object graph)
        {
            Travel(visitor, (NullableValuesEntity) graph);
        }

        public void Travel(IReadVisitor visitor, object graph)
        {
            Travel(visitor, (NullableValuesEntity)graph);
        }

        public void Travel(IWriteVisitor visitor, NullableValuesEntity graph)
        {
            //visitor.VisitValue(new int?(graph.Id), WriteVisitArgs.Value("Id", 1u));
            //visitor.VisitValue(graph.MayBool, WriteVisitArgs.NullableValue("MayBool", 2u, graph.MayBool.HasValue));
            visitor.VisitValue(graph.MayInt, WriteVisitArgs.NullableValue("MayInt", 3u, graph.MayInt.HasValue));
            //visitor.VisitValue(graph.MayDateTime, WriteVisitArgs.NullableValue("MayDateTime", 4u, graph.MayDateTime.HasValue));
            //visitor.VisitValue(graph.MayTimeSpan, WriteVisitArgs.NullableValue("MayTimeSpan", 5u, graph.MayTimeSpan.HasValue));
        }

        public void Travel(IReadVisitor visitor, NullableValuesEntity graph)
        {
        	int? num;
	        if (visitor.TryVisitValue(ReadVisitArgs.NullableValue("MayInt", 1u), out num))
		        graph.MayInt = num;
        }
    }
}
