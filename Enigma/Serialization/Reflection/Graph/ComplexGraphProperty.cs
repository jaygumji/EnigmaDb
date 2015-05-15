using System;

namespace Enigma.Serialization.Reflection.Graph
{
    public class ComplexGraphProperty : IGraphProperty
    {
        private readonly SerializableProperty _property;
        private readonly IGraphType _propertyType;
        private readonly VisitArgs _args;

        public ComplexGraphProperty(SerializableProperty property, IGraphType propertyType)
        {
            _property = property;
            _propertyType = propertyType;
            _args = property.CreateVisitArgs();
        }

        public void Visit(object graph, IReadVisitor visitor)
        {
            var state = visitor.TryVisit(_args);
            if (state == ValueState.NotFound) return;

            if (state == ValueState.Null) {
                _property.Ref.SetValue(graph, null);
                visitor.Leave(_args);
                return;
            }

            var childGraph = Activator.CreateInstance(_property.Ref.PropertyType);

            _propertyType.Visit(childGraph, visitor);

            _property.Ref.SetValue(graph, childGraph);
            visitor.Leave(_args);
        }

        public void Visit(object graph, IWriteVisitor visitor)
        {
            var childGraph = _property.Ref.GetValue(graph);
            visitor.Visit(childGraph, _args);
            if (childGraph == null) {
                visitor.Leave(null, _args);
                return;
            }

            _propertyType.Visit(childGraph, visitor);
            visitor.Leave(childGraph, _args);
        }
    }
}