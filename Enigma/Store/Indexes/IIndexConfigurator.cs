using Enigma.IO;
using System.Collections.Generic;

namespace Enigma.Store.Indexes
{
    public interface IIndexConfigurator
    {
        IEnumerable<IndexConfiguration> Indexes { get; }

        IStreamProvider GetStreamProvider(string name);
    }
}
