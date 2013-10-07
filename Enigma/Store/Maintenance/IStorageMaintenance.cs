namespace Enigma.Store.Maintenance
{
    public interface IStorageMaintenance
    {
        bool IsFragmented { get; }
        MaintenanceMode Mode { get; }

        void Truncate();
    }
}
