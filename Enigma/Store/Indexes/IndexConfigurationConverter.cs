using Enigma.Modelling;
using System.Collections.Generic;
namespace Enigma.Store.Indexes
{
    public class IndexConfigurationConverter
    {

        private readonly IEntityMap _primaryEntity;
        private readonly Model _model;

        public IndexConfigurationConverter(Model model, IEntityMap primaryEntity)
        {
            _model = model;
            _primaryEntity = primaryEntity;
        }

        public List<IndexConfiguration> Convert()
        {
            var list = new List<IndexConfiguration>();

            ConvertRecursive(list, _primaryEntity, null);

            return list;
        }

        private void ConvertRecursive(List<IndexConfiguration> list, IEntityMap entityMap, string left)
        {
            foreach (var index in entityMap.Indexes) {
                list.Add(new IndexConfiguration {
                    UniqueName = CreatePath(left, index.PropertyName),
                    Type = index.ValueType,
                    EntityType = entityMap.EntityType
                });
            }

            foreach (var property in entityMap.Properties) {
                // TODO: Create sub indexes recursively
            }
        }

        private string CreatePath(string left, string right)
        {
            if (string.IsNullOrEmpty(left)) return right;
            return string.Concat(left, '.', right);
        }

    }
}
