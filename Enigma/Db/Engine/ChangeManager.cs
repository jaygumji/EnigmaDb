using Enigma.Modelling;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Enigma.Db
{
    public class ChangeManager : IChangeManager
    {
        private readonly Dictionary<Type, IChangeEntry> _entries;
        private readonly Model _model;

        public ChangeManager(Model model)
        {
            _entries = new Dictionary<Type, IChangeEntry>();
            _model = model;
        }

        public Model Model { get { return _model; } }

        public ChangeEntry<T> GetEntry<T>()
        {
            IChangeEntry entry;
            if (_entries.TryGetValue(typeof(T), out entry))
                return (ChangeEntry<T>)entry;

            var newEntry = new ChangeEntry<T>();
            _entries.Add(typeof(T), newEntry);
            return newEntry;
        }

        public void Add<T>(T entity)
        {
            GetEntry<T>().Add(entity);
        }

        public void Update<T>(T entity)
        {
            GetEntry<T>().Update(entity);
        }

        public void Remove<T>(T entity)
        {
            GetEntry<T>().Remove(entity);
        }

        public int SaveChanges(IEnigmaEngine engine)
        {
            return _entries.Values.Sum(e => e.SaveChanges(engine));
        }
    }
}
