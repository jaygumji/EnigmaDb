using System;
using Enigma.Serialization.Reflection.Emit;

namespace Enigma.Serialization
{
    public class SerializationEngine
    {

        private static readonly DynamicTravellerContext Context = new DynamicTravellerContext();
        private static readonly object[] EmptyParameters = {};

        public void Serialize(IWriteVisitor visitor, object graph)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");
            if (graph == null) throw new ArgumentNullException("graph");
            var type = graph.GetType();

            var traveller = Context.GetInstance(type);
            traveller.Travel(visitor, graph);
        }

        public void Serialize<T>(IWriteVisitor visitor, T graph)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");
            if (graph == null) throw new ArgumentNullException("graph");
            var type = graph.GetType();

            var traveller = Context.GetInstance(type);
            traveller.Travel(visitor, graph);
        }

        public object Deserialize(IReadVisitor visitor, Type type)
        {
            if (visitor == null) throw new ArgumentNullException("visitor");
            if (type == null) throw new ArgumentNullException("type");
            
            if (visitor.TryVisit(ReadVisitArgs.Root(type.Name)) != ValueState.Found)
                return null;

            var constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw InvalidGraphException.NoParameterLessConstructor(type);
            var graph = constructor.Invoke(EmptyParameters);

            var traveller = Context.GetInstance(type);
            traveller.Travel(visitor, graph);

            visitor.Leave();

            return graph;
        }

        public T Deserialize<T>(IReadVisitor visitor)
        {
            var type = typeof (T);
            if (visitor == null) throw new ArgumentNullException("visitor");

            if (visitor.TryVisit(ReadVisitArgs.Root(type.Name)) != ValueState.Found)
                return default(T);

            var constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw InvalidGraphException.NoParameterLessConstructor(type);
            var graph = (T) constructor.Invoke(EmptyParameters);

            var traveller = Context.GetInstance<T>();
            traveller.Travel(visitor, graph);

            visitor.Leave();

            return graph;
        }

    }
}
