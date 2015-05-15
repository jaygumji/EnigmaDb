using System;

namespace Enigma.Serialization.Reflection.Graph
{
    public class Int16GraphProperty : IGraphProperty
    {

        private readonly SerializableProperty _property;
        private readonly VisitArgs _args;

        public Int16GraphProperty(SerializableProperty property)
        {
            _property = property;
            _args = property.CreateVisitArgs();
        }

        public void Visit(object graph, IReadVisitor visitor)
        {
            Int16? value;
            if (visitor.TryVisitValue(_args, out value) && value.HasValue)
                _property.Ref.SetValue(graph, value.Value);
        }

        public void Visit(object graph, IWriteVisitor visitor)
        {
            var value = (Int16) _property.Ref.GetValue(graph);
            visitor.VisitValue(value, _args);
        }
    }
}