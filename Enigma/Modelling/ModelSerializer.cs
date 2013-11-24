using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Linq;

namespace Enigma.Modelling
{
    public class ModelSerializer
    {

        private readonly Type _entityMapType = typeof(EntityMap<>);
        private readonly Type[] _entityMapConstructorArguments = new[] { typeof(Dictionary<string, IPropertyMap>), typeof(string), typeof(string) };

        private readonly Type _propertyMapType = typeof(PropertyMap<>);
        private readonly Type[] _propertyMapConstructorArguments = new[] { typeof(string), typeof(string), typeof(int) };

        private IEntityMap CreateEntityMap(Type entityType, IEnumerable<IPropertyMap> properties, string entityName, string keyName)
        {
            var constructor = _entityMapType.MakeGenericType(entityType).GetConstructor(_entityMapConstructorArguments);
            return (IEntityMap)constructor.Invoke(new object[] {properties.ToDictionary(p => p.PropertyName), entityName, keyName});
        }

        private IPropertyMap CreatePropertyMap(Type propertyType, string propertyName, string name, int index)
        {
            var constructor = _propertyMapType.MakeGenericType(propertyType).GetConstructor(_propertyMapConstructorArguments);
            return (IPropertyMap)constructor.Invoke(new object[] {propertyName, name, index});
        }

        public Model Load(Stream stream)
        {
            var entityMaps = new List<IEntityMap>();
            var xmlReader = XmlDictionaryReader.CreateTextReader(stream, Encoding.UTF8, XmlDictionaryReaderQuotas.Max, null);
            if (!xmlReader.ReadToFollowing("Model"))
                throw new ArgumentException("Missing required element Model");

            var visitor = new XmlVisitor(xmlReader);

            while (visitor.VisitNext())
            {
                if (visitor.NodeType != XmlNodeType.Element) continue;
                if (visitor.Name != "Entities")
                    throw new ArgumentException("Unexpected xml element " + visitor.Name);

                if (!visitor.IsEmptyElement)
                    ReadEntityMaps(visitor, entityMaps);
            }
            return new Model(entityMaps);
        }

        private void ReadEntityMaps(XmlVisitor visitor, List<IEntityMap> entityMaps)
        {
            while (visitor.VisitNext())
            {
                if (visitor.NodeType == XmlNodeType.EndElement && visitor.Name == "Entities")
                    return;

                if (visitor.NodeType != XmlNodeType.Element) continue;

                if (visitor.Name == "Entity")
                    entityMaps.Add(ReadEntityMap(visitor));
                else
                    throw new ArgumentException("Unexpected xml element " + visitor.Name);
            }
        }

        private IEntityMap ReadEntityMap(XmlVisitor visitor)
        {
            Type entityType = null;
            string entityName = null;
            string keyName = null;
            var propertyIndexLength = 0;
            var properties = new List<IPropertyMap>();
            var indexes = new List<IIndex>();

            while (visitor.VisitNext())
            {
                if (visitor.NodeType == XmlNodeType.EndElement && visitor.Name == "Entity")
                {
                    if (entityType == null) throw new ArgumentException("Missing entity type");
                    if (entityName == null) entityName = entityType.Name;

                    var entityMap = EntityMap.Create(entityType, properties, indexes);
                    entityMap.KeyName = keyName;
                    entityMap.PropertyIndexLength = propertyIndexLength;
                    return entityMap;
                }

                if (visitor.NodeType != XmlNodeType.Element) continue;

                if (visitor.Name == "Type")
                    entityType = Type.GetType(visitor.GetElementContentAsString());
                else if (visitor.Name == "Name")
                    entityName = visitor.GetElementContentAsString();
                else if (visitor.Name == "KeyName")
                    keyName = visitor.GetElementContentAsString();
                else if (visitor.Name == "PropertyIndexLength")
                    propertyIndexLength = visitor.GetElementContentAsInt32();
                else if (visitor.Name == "Properties")
                {
                    if (!visitor.IsEmptyElement)
                        ReadPropertyMaps(visitor, entityType, properties);
                }
                else if (visitor.Name == "Indexes")
                {
                    if (!visitor.IsEmptyElement)
                        ReadIndexes(visitor, entityType, indexes);
                }
                else
                    throw new ArgumentException("Unexpected xml element " + visitor.Name);
            }
            throw new ArgumentException("Unexpected end of stream");
        }

        private void ReadIndexes(XmlVisitor visitor, Type entityType, List<IIndex> indexes)
        {
            while (visitor.VisitNext())
            {
                if (visitor.NodeType == XmlNodeType.EndElement && visitor.Name == "Indexes")
                    return;

                if (visitor.NodeType != XmlNodeType.Element) continue;

                if (visitor.Name == "Index")
                    indexes.Add(ReadIndex(visitor, entityType));
                else
                    throw new ArgumentException("Unexpected xml element " + visitor.Name);
            }
        }

        private IIndex ReadIndex(XmlVisitor visitor, Type entityType)
        {
            string uniqueName = null;
            while (visitor.VisitNext())
            {
                if (visitor.NodeType == XmlNodeType.EndElement && visitor.Name == "Index")
                {
                    if (string.IsNullOrEmpty(uniqueName)) throw new ArgumentException("Missing required element UniqueName");

                    return Index.Create(entityType, uniqueName);
                }

                if (visitor.NodeType != XmlNodeType.Element) continue;

                // TODO: Backwardscompatiblitiy 0.2.0: PropertyName
                if (string.Equals(visitor.Name, "PropertyName", StringComparison.InvariantCulture))
                    uniqueName = visitor.GetElementContentAsString();
                else if (string.Equals(visitor.Name, "UniqueName", StringComparison.InvariantCulture))
                    uniqueName = visitor.GetElementContentAsString();
                else
                    throw new ArgumentException("Unexpected xml element " + visitor.Name);
            }
            throw new ArgumentException("Unexpected end of stream");
        }

        private void ReadPropertyMaps(XmlVisitor visitor, Type entityType, List<IPropertyMap> properties)
        {
            while (visitor.VisitNext())
            {
                if (visitor.NodeType == XmlNodeType.EndElement && visitor.Name == "Properties")
                    return;

                if (visitor.NodeType != XmlNodeType.Element) continue;

                if (visitor.Name == "Property")
                    properties.Add(ReadPropertyMap(visitor, entityType));
                else
                    throw new ArgumentException("Unexpected xml element " + visitor.Name);
            }
        }

        private IPropertyMap ReadPropertyMap(XmlVisitor visitor, Type entityType)
        {
            string name = null;
            string propertyName = null;
            int index = -1;
            while (visitor.VisitNext())
            {
                if (visitor.NodeType == XmlNodeType.EndElement && visitor.Name == "Property")
                {
                    if (string.IsNullOrEmpty(name)) throw new ArgumentException("Missing required element Property/Name");
                    if (index < 0) throw new ArgumentException("Missing required element Property/Index");
                    if (string.IsNullOrEmpty(propertyName)) propertyName = name;

                    var propertyMap = PropertyMap.Create(entityType, propertyName);
                    propertyMap.Name = name;
                    propertyMap.Index = index;
                    return propertyMap;
                }

                if (visitor.NodeType != XmlNodeType.Element) continue;

                if (visitor.Name == "Name")
                    name = visitor.GetElementContentAsString();
                else if (visitor.Name == "UniqueName")
                    propertyName = visitor.GetElementContentAsString();
                else if (visitor.Name == "Index")
                    index = visitor.GetElementContentAsInt32();
                else
                    throw new ArgumentException("Unexpected xml element " + visitor.Name);
            }
            throw new ArgumentException("Unexpected end of stream");
        }

        public void Save(Model model, Stream stream)
        {
            using (var xmlWriter = XmlDictionaryWriter.CreateTextWriter(stream, Encoding.UTF8, false))
            {
                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("Model");
                xmlWriter.WriteStartElement("Entities");
                foreach (var entityMap in model.EntityMaps)
                {
                    xmlWriter.WriteStartElement("Entity");
                    xmlWriter.WriteElementString("Type", entityMap.EntityType.AssemblyQualifiedName);
                    xmlWriter.WriteElementString("Name", entityMap.Name);
                    xmlWriter.WriteElementString("KeyName", entityMap.KeyName);
                    xmlWriter.WriteStartElement("PropertyIndexLength");
                    xmlWriter.WriteValue(((EntityMap) entityMap).PropertyIndexLength);
                    xmlWriter.WriteEndElement(); // PropertyIndexLength

                    xmlWriter.WriteStartElement("Properties");
                    foreach (var property in entityMap.Properties)
                    {
                        xmlWriter.WriteStartElement("Property");
                        xmlWriter.WriteElementString("Name", property.Name);
                        if (property.Name != property.PropertyName)
                            xmlWriter.WriteElementString("UniqueName", property.PropertyName);
                        xmlWriter.WriteStartElement("Index");
                        xmlWriter.WriteValue(property.Index);
                        xmlWriter.WriteEndElement(); // Index
                        xmlWriter.WriteEndElement(); // Property
                    }
                    xmlWriter.WriteEndElement(); // Properties

                    xmlWriter.WriteStartElement("Indexes");
                    foreach (var index in entityMap.Indexes)
                    {
                        xmlWriter.WriteStartElement("Index");
                        xmlWriter.WriteElementString("UniqueName", index.UniqueName);
                        xmlWriter.WriteEndElement(); // Index
                    }
                    xmlWriter.WriteEndElement(); // Indexes

                    xmlWriter.WriteEndElement(); // Entity
                }
                xmlWriter.WriteEndElement(); // Entities
                xmlWriter.WriteEndDocument();
            }
        }

    }
}
