using Enigma.Store.Indexes;
using System.Collections.Generic;
namespace Enigma.Store
{
    public interface ICompositeStorageConfigurator
    {
        CompositeStorageConfiguration Configuration { get; }

        IEnumerable<IStorageFragment> GetFragments();
        IStorageFragment CreateFragment();
    }
}
