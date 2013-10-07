using Enigma.Modelling;
namespace Enigma.Db
{
    public interface IChangeManager : IChangeTracker
    {
        Model Model { get; }

        void Add<T>(T entity);
        void Update<T>(T entity);
        void Remove<T>(T entity);

        int SaveChanges(IEnigmaEngine engine);
    }
}
