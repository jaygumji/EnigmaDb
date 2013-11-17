using System;
using System.Collections.Concurrent;
using Enigma.Modelling;
using ProtoBuf.Meta;

namespace Enigma.ProtocolBuffer
{
    internal static class TypeModelBuilder
    {
        
        private static readonly ConcurrentDictionary<Model, TypeModel> CompiledModels;
        private static readonly ConcurrentDictionary<Type, TypeModel> CompiledTypes;

        static TypeModelBuilder()
        {
            CompiledModels = new ConcurrentDictionary<Model, TypeModel>();
            CompiledTypes = new ConcurrentDictionary<Type, TypeModel>();
        }

        public static TypeModel Create<T>()
        {
            return CompiledTypes.GetOrAdd(typeof (T), type => CreateTypeModel(Model.ByConvention<T>()));
        }

        public static TypeModel Create(Model model)
        {
            return CompiledModels.GetOrAdd(model, CreateTypeModel);
        }

        private static TypeModel CreateTypeModel(Model model)
        {
            var typeModel = TypeModel.Create();
            foreach (var entityMap in model.EntityMaps)
                ApplyEntityMap(typeModel, entityMap);

            return typeModel.Compile();
        }

        private static void ApplyEntityMap(RuntimeTypeModel typeModel, IEntityMap entityMap)
        {
            var metaType = typeModel.Add(entityMap.EntityType, false);
            foreach (var propertyMap in entityMap.Properties)
                metaType.Add(propertyMap.Index, propertyMap.PropertyName);
        }

    }
}