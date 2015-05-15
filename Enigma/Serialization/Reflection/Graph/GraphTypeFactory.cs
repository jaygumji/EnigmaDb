using System;
using System.Collections.Generic;
using Enigma.Reflection;

namespace Enigma.Serialization.Reflection.Graph
{
    public class GraphTypeFactory
    {
        private static readonly Dictionary<Type, Func<SerializableProperty, IGraphProperty>> PredefinedGraphPropertyFactories
            = new Dictionary<Type, Func<SerializableProperty, IGraphProperty>> {
            {typeof (Int16), (ser) => new Int16GraphProperty(ser)}
        }; 

        private readonly SerializableTypeProvider _provider;
        private readonly Dictionary<Type, IGraphType> _graphTypes;

        public GraphTypeFactory(SerializableTypeProvider provider)
        {
            _provider = provider;
            _graphTypes = new Dictionary<Type, IGraphType>();
        }

        public IGraphType GetOrCreate(Type type)
        {
            var serType = _provider.GetOrCreate(type);

            var graphProperties = new List<IGraphProperty>();
            var properties = serType.Properties;
            foreach (var property in properties) {
                var graphProperty = Create(property);
                graphProperties.Add(graphProperty);
            }

            var graphType = new ComplexGraphType(graphProperties);
            _graphTypes.Add(type, graphType);
            return graphType;
        }

        private IGraphProperty Create(SerializableProperty ser)
        {
            Func<SerializableProperty, IGraphProperty> factory;
            if (PredefinedGraphPropertyFactories.TryGetValue(ser.Ref.PropertyType, out factory))
                return factory(ser);

            DictionaryContainerTypeInfo dictionaryTypeInfo;
            if (ser.Ext.TryGetDictionaryTypeInfo(out dictionaryTypeInfo)) {
                
            }

            CollectionContainerTypeInfo collectionTypeInfo;
            if (ser.Ext.TryGetCollectionTypeInfo(out collectionTypeInfo)) {
                
            }

            return new ComplexGraphProperty(ser, GetOrCreate(ser.Ref.PropertyType));

            //throw new ArgumentException(string.Format("Could not create a graph property of property {0} with type {1}", ser.Ref.Name, ser.Ref.PropertyType.FullName));
        }

    }
}