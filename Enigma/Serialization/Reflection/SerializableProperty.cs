using System.Reflection;
using Enigma.Reflection;

namespace Enigma.Serialization.Reflection
{
    public class SerializableProperty
    {
        private readonly PropertyInfo _ref;
        private readonly SerializationMetadata _metadata;
        private readonly ExtendedType _ext;

        public SerializableProperty(PropertyInfo @ref, SerializationMetadata metadata)
        {
            _ref = @ref;
            _metadata = metadata;
            _ext = _ref.PropertyType.Extend();
        }

        public PropertyInfo Ref
        {
            get { return _ref; }
        }

        public SerializationMetadata Metadata
        {
            get { return _metadata; }
        }

        public ExtendedType Ext
        {
            get { return _ext; }
        }

        public VisitArgs CreateVisitArgs()
        {
            if (_ext.Class == TypeClass.Dictionary)
                return VisitArgs.Dictionary(_ref.Name, _metadata);

            if (_ext.Class == TypeClass.Collection)
                return VisitArgs.Collection(_ref.Name, _metadata);

            if (_ext.Class == TypeClass.Complex)
                return VisitArgs.Single(_ref.Name, _metadata);

            return VisitArgs.Value(_ref.Name, _metadata);
        }
    }
}