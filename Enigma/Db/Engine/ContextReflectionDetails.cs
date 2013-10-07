using Enigma.Modelling;
using System.Collections.Generic;
using System.Reflection;

namespace Enigma.Db.Engine
{
    class ContextReflectionDetails
    {
        private readonly Model _model;
        private readonly List<PropertyInfo> _setProperties;

        public ContextReflectionDetails(Model model, List<PropertyInfo> setProperties)
        {
            _model = model;
            _setProperties = setProperties;
        }

        public Model Model { get { return _model; } }
        public IEnumerable<PropertyInfo> SetProperties { get { return _setProperties; } }

    }
}
