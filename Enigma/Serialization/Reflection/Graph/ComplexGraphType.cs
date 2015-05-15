using System.Collections.Generic;

namespace Enigma.Serialization.Reflection.Graph
{
    public class ComplexGraphType : IGraphType
    {
        private readonly IEnumerable<IGraphProperty> _properties;

        public ComplexGraphType(IEnumerable<IGraphProperty> properties)
        {
            _properties = properties;
        }

        public IEnumerable<IGraphProperty> Properties
        {
            get { return _properties; }
        }

        public void Visit(object graph, IReadVisitor visitor)
        {
            foreach (var property in _properties)
                property.Visit(graph, visitor);
        }

        public void Visit(object graph, IWriteVisitor visitor)
        {
            foreach (var property in _properties)
                property.Visit(graph, visitor);
        }
    }
}