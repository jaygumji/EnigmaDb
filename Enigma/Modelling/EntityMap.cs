using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Enigma.Modelling
{
    public class EntityMap<T> : EntityMap, IEntityMap<T>
    {
        public EntityMap()
            : base(typeof(T))
        {
        }

        public EntityMap(IEnumerable<IPropertyMap> propertyMappings, IEnumerable<IIndex> indexes)
            : base(typeof(T), propertyMappings, indexes)
        {
        }

        public PropertyMap<TProperty> Property<TProperty>(Expression<Func<T, TProperty>> property)
        {
            var propertyInfo = Expressions.PropertySelector.GetProperty<T, TProperty>(property);
            IPropertyMap propertyMap;
            if (TryGetProperty(propertyInfo.Name, out propertyMap))
                return (PropertyMap<TProperty>)propertyMap;

            var newPropertyMap = new PropertyMap<TProperty>(propertyInfo, ++PropertyIndexLength);
            Add(newPropertyMap);
            return newPropertyMap;
        }

        public Index<TProperty> Index<TProperty>(Expression<Func<T, TProperty>> property)
        {
            var propertyInfo = Expressions.PropertySelector.GetProperty<T, TProperty>(property);
            IIndex index;
            if (TryGetIndex(propertyInfo.Name, out index))
                return (Index<TProperty>)index;

            var newIndex = new Index<TProperty>(propertyInfo);
            Add(newIndex);
            return newIndex;
        }
    }

    public abstract class EntityMap : IEntityMap
    {
        private readonly Type _entityType;
        private readonly Dictionary<string, IPropertyMap> _properties;
        private readonly Dictionary<string, IIndex> _indexes;
        public string KeyName { get; set; }
        public int PropertyIndexLength { get; set; }

        protected EntityMap(Type entityType)
            : this(entityType, new Dictionary<string, IPropertyMap>(), new Dictionary<string, IIndex>())
        {
        }

        protected EntityMap(Type entityType, IEnumerable<IPropertyMap> propertyMappings, IEnumerable<IIndex> indexes)
            : this(entityType, propertyMappings.ToDictionary(pm => pm.PropertyName), indexes.ToDictionary(i => i.PropertyName))
        {
        }

        protected EntityMap(Type entityType, Dictionary<string, IPropertyMap> propertyMappings, Dictionary<string, IIndex> indexes)
        {
            _entityType = entityType;
            _properties = propertyMappings;
            _indexes = indexes;

            PropertyIndexLength = propertyMappings.Count == 0 ? 0 : propertyMappings.Values.Max(pm => pm.Index);
        }

        public static EntityMap Create(Type entityType, IEnumerable<IPropertyMap> propertyMappings, IEnumerable<IIndex> indexes)
        {
            var type = typeof(EntityMap<>).MakeGenericType(entityType);
            return (EntityMap) Activator.CreateInstance(type, propertyMappings, indexes);
        }

        public bool TryGetProperty(string propertyName, out IPropertyMap propertyMap)
        {
            return _properties.TryGetValue(propertyName, out propertyMap);
        }

        public void Add(IPropertyMap propertyMap)
        {
            _properties.Add(propertyMap.PropertyName, propertyMap);
        }

        public bool TryGetIndex(string propertyName, out IIndex index)
        {
            return _indexes.TryGetValue(propertyName, out index);
        }

        public void Add(IIndex index)
        {
            _indexes.Add(index.PropertyName, index);
        }

        public string Name { get { return _entityType.Name; } }
        public Type EntityType { get { return _entityType; } }
        public IEnumerable<IPropertyMap> Properties { get { return _properties.Values; } }
        public IEnumerable<IIndex> Indexes { get { return _indexes.Values; } }

        public PropertyInfo GetKeyProperty()
        {
            return EntityType.GetProperty(KeyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }

        public override int GetHashCode()
        {
            var hashCode = _properties.Count ^ _indexes.Count ^ Name.GetHashCode();
            foreach (var property in _properties.Values)
                hashCode ^= property.GetHashCode();

            return hashCode;
        }

        public void CopyFrom(EntityMap entity)
        {
            PropertyIndexLength = entity.PropertyIndexLength;
            foreach (var property in _properties.Values.OrderBy(p => p.Index))
            {
                IPropertyMap otherPropertyMap;
                if (entity._properties.TryGetValue(property.PropertyName, out otherPropertyMap))
                    ((PropertyMap)property).CopyFrom((PropertyMap)otherPropertyMap);
                else if (property.Index <= PropertyIndexLength)
                    property.Index = ++PropertyIndexLength;
                else
                    PropertyIndexLength = property.Index;
            }
            foreach (var index in entity._indexes.Values)
            {
                IIndex myIndex;
                if (_indexes.TryGetValue(index.PropertyName, out myIndex))
                    ((Index)myIndex).CopyFrom((Index)index);
            }
        }
    }

}
