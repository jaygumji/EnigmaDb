using Enigma.Serialization;
using Enigma.Test.Serialization.Fakes;

namespace Enigma.Test.Serialization.HardCoded
{
    public class NullableValuesEntityHardCodedTraveller : IGraphTraveller<NullableValuesEntity>
    {
        private readonly VisitArgs _argsId0;
        private readonly VisitArgs _argsMayBool1;
        private readonly VisitArgs _argsMayInt2;
        private readonly VisitArgs _argsMayDateTime3;
        private readonly VisitArgs _argsMayTimeSpan4;

        public NullableValuesEntityHardCodedTraveller(IVisitArgsFactory factory)
        {
            _argsId0 = factory.Construct("Id");
            _argsMayBool1 = factory.Construct("MayBool");
            _argsMayInt2 = factory.Construct("MayInt");
            _argsMayDateTime3 = factory.Construct("MayDateTime");
            _argsMayTimeSpan4 = factory.Construct("MayTimeSpan");
        }

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
            //visitor.VisitValue(new int?(graph.Id), _argsId0);
            //visitor.VisitValue(graph.MayBool, _argsMayBool1);
            visitor.VisitValue(graph.MayInt, _argsMayInt2);
            //visitor.VisitValue(graph.MayDateTime, _argsMayDateTime3);
            //visitor.VisitValue(graph.MayTimeSpan, _argsMayTimeSpan4);
        }

        public void Travel(IReadVisitor visitor, NullableValuesEntity graph)
        {
        	int? num;
	        if (visitor.TryVisitValue(_argsMayInt2, out num))
		        graph.MayInt = num;
        }
    }
}
