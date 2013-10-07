using System.Collections.Generic;
using System.Linq;

namespace Enigma.Store.Maintenance
{
    public class CompositeStorageMaintenance : IStorageMaintenance
    {

        private readonly StorageFragmentCollection _fragments;

        public CompositeStorageMaintenance(StorageFragmentCollection fragments)
        {
            _fragments = fragments;
            Mode = MaintenanceMode.Normal;
        }

        public bool IsFragmented
        {
            get { return _fragments.Any(f => f.Maintenance.IsFragmented); }
        }

        public MaintenanceMode Mode { get; private set; }

        public void Truncate()
        {
            Mode = MaintenanceMode.Lockdown;

            foreach (var fragment in _fragments)
                fragment.Maintenance.Truncate();

            Mode = MaintenanceMode.Normal;
        }

    }
}
