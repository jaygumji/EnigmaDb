using Enigma.Modelling;
using Enigma.Store.Indexes;
namespace Enigma.Store
{
    public interface IStorageFactoryService
    {
        Model Model { get; }

        IStorage CreateStorage(string name);
        IIndexCollection CreateIndexes(string name);

        void SynchronizeModel(Model model);

    }
}
