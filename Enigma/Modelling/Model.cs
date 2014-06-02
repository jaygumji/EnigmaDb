using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Linq;
using System;
using System.Reflection;
using Enigma.Reflection;

namespace Enigma.Modelling
{
    public class Model
    {
        private readonly Dictionary<string, IEntityMap> _entityMaps;

        public Model()
        {
            _entityMaps = new Dictionary<string, IEntityMap>();
        }

        public Model(IEnumerable<IEntityMap> entityMaps)
        {
            _entityMaps = entityMaps.ToDictionary(em => em.Name);
        }

        public static Model Load(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                return Load(fileStream);
        }

        public static Model Load(Stream stream)
        {
            var serializer = new ModelSerializer();
            return serializer.Load(stream);
        }

        public IEnumerable<IEntityMap> EntityMaps { get { return _entityMaps.Values; } }

        public void Save(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                Save(fileStream);
        }

        public void Save(Stream stream)
        {
            var serializer = new ModelSerializer();
            serializer.Save(this, stream);
        }

        public EntityMap<T> Entity<T>()
        {
            IEntityMap entityMap;
            if (_entityMaps.TryGetValue(typeof(T).Name, out entityMap))
                return (EntityMap<T>)entityMap;

            var newEntityMap = new EntityMap<T>();
            _entityMaps.Add(newEntityMap.Name, newEntityMap);
            return newEntityMap;
        }

        public void Register(Type entityType)
        {
            RegisterHierarchy(entityType);
        }

        private void RegisterHierarchy(Type entityType)
        {
            RegisterHierarchy(entityType, entityType.Name);
        }

        private void RegisterHierarchy(Type entityType, string name)
        {
            if (_entityMaps.ContainsKey(name))
                return;

            var properties = entityType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var propertyMappings = new Dictionary<string, IPropertyMap>();
            var propertyMapType = typeof(PropertyMap<>);

            var relationTypes = new List<Type>();
            foreach (var property in properties.Where(p => p.CanRead && p.CanWrite).OrderBy(p => p.Name))
            {
                if (!propertyMappings.ContainsKey(property.Name))
                {
                    var genericPropertyMapType = propertyMapType.MakeGenericType(property.PropertyType);
                    var propertyIndex = propertyMappings.Count + 1;
                    var propertyMap = (IPropertyMap)Activator.CreateInstance(genericPropertyMapType, property, propertyIndex);
                    propertyMappings.Add(property.Name, propertyMap);

                    var extended = new ExtendedType(property.PropertyType);
                    if (extended.Class == TypeClass.Collection) {
                        var collectionInfo = extended.Container.AsCollection();
                        var extendedElementType = new ExtendedType(collectionInfo.ElementType);
                        if (extendedElementType.Class == TypeClass.Complex)
                            relationTypes.Add(collectionInfo.ElementType);
                    }
                    else if (extended.Class == TypeClass.Dictionary) {
                        var dictionaryInfo = extended.Container.AsDictionary();
                        var extendedKeyType = new ExtendedType(dictionaryInfo.KeyType);
                        if (extendedKeyType.Class == TypeClass.Complex)
                            relationTypes.Add(dictionaryInfo.KeyType);

                        var extendedValueType = new ExtendedType(dictionaryInfo.ValueType);
                        if (extendedValueType.Class == TypeClass.Complex)
                            relationTypes.Add(dictionaryInfo.ValueType);
                    }
                    else if (extended.Class == TypeClass.Complex)
                        relationTypes.Add(property.PropertyType);
                }
            }

            var entityMap = EntityMap.Create(entityType, propertyMappings.Values, new IIndex[] {});

            if (propertyMappings.ContainsKey("Id"))
                entityMap.KeyName = "Id";
            else if (propertyMappings.ContainsKey("ID"))
                entityMap.KeyName = "ID";
            else if (propertyMappings.ContainsKey(entityType.Name + "Id"))
                entityMap.KeyName = entityType.Name + "Id";
            else if (propertyMappings.ContainsKey(entityType.Name + "ID"))
                entityMap.KeyName = entityType.Name + "ID";
            else if (propertyMappings.ContainsKey("Guid"))
                entityMap.KeyName = "Guid";
            else if (propertyMappings.ContainsKey("GUID"))
                entityMap.KeyName = "GUID";

            _entityMaps.Add(name, entityMap);

            foreach (var relationType in relationTypes)
                RegisterHierarchy(relationType);
        }

        public IEntityMap GetEntity(string name)
        {
            return _entityMaps[name];
        }

        public IEntityMap GetEntity(Type entityType)
        {
            return _entityMaps[entityType.Name];
        }

        public static Model ByConvention<T>()
        {
            var model = new Model();
            var entityType = typeof(T);
            model.Register(entityType);
            return model;
        }

        public void CopyFrom(Model model)
        {
            foreach (var entity in model._entityMaps.Values)
            {
                IEntityMap myEntityMap;
                if (_entityMaps.TryGetValue(entity.Name, out myEntityMap))
                    ((EntityMap)myEntityMap).CopyFrom((EntityMap)entity);
            }
        }

        public override int GetHashCode()
        {
            var hashCode = _entityMaps.Count;
            foreach (var entity in _entityMaps.Values)
                hashCode ^= entity.GetHashCode();

            return hashCode;
        }
    }
}
