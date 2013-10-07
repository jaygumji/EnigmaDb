using System.Collections.Generic;
namespace Enigma.Db
{
    public class ChangeEntry<T> : IChangeEntry
    {
        private readonly List<T> _added;
        private readonly List<T> _modified;
        private readonly List<T> _removed;

        public ChangeEntry()
        {
            _added = new List<T>();
            _modified = new List<T>();
            _removed = new List<T>();
        }

        public void Add(T entity)
        {
            _added.Add(entity);
        }

        public void Remove(T entity)
        {
            _removed.Add(entity);
        }

        public void Update(T entity)
        {
            _modified.Add(entity);
        }

        public int SaveChanges(IEnigmaEngine engine)
        {
            var entityEngine = engine.GetEntityEngine<T>();
            var count = 0;
            foreach (var entity in _added)
            {
                if (entityEngine.TryAdd(entity))
                    count++;
            }

            foreach (var entity in _modified)
            {
                if (entityEngine.TryUpdate(entity))
                    count++;
            }

            foreach (var entity in _removed)
            {
                if (entityEngine.TryRemove(entity))
                    count++;
            }

            if (count > 0)
                entityEngine.CommitModifications();

            return count;
        }

    }
}
