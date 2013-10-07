using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Enigma.Modelling
{
    public class PropertyBuilder<T>
    {
        private EntityMap _entityMap;
        private PropertyMap<T> _propertyMap;

        public PropertyBuilder(EntityMap entityMap, PropertyMap<T> propertyMap)
        {
            _entityMap = entityMap;
            _propertyMap = propertyMap;
        }

        public PropertyBuilder<T> Name(string newName)
        {
            _propertyMap.Name = newName;
            return this;
        }

        public PropertyBuilder<T> Index(int newIndex)
        {
            _propertyMap.Index = newIndex;
            if (_entityMap.PropertyIndexLength < newIndex)
                _entityMap.PropertyIndexLength = newIndex;
            return this;
        }
    }
}
