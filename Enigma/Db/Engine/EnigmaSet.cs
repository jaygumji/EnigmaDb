using Enigma.Db.Linq;
using Enigma.Entities;
using Enigma.Store;
using Enigma.Store.Keys;
using System;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;

namespace Enigma.Db
{

    public class EnigmaSet<T> : ISet<T>
    {
        private readonly IEnigmaEntityEngine<T> _engine;
        private readonly IChangeManager _changeManager;
        private readonly IQueryable<T> _queryable;
        
        public EnigmaSet(IEnigmaEngine engine, IChangeManager changeManager)
        {
            _engine = engine.GetEntityEngine<T>();
            _changeManager = changeManager;
            _queryable = new EnigmaQueryable<T>(engine, changeManager.Model);
        }

        public void Add(T entity)
        {
            _changeManager.Add(entity);
        }

        public void Remove(T entity)
        {
            _changeManager.Remove(entity);
        }

        public T Get(object id)
        {
            T entity;
            if (!TryGet(id, out entity))
                throw new EntityNotFoundException(id);

            return entity;
        }

        public bool TryGet(object id, out T entity)
        {
            if (_engine.TryGet(id, out entity))
            {
                _changeManager.Update(entity);
                return true;
            }
            return false;
        }


        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return _queryable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        Type IQueryable.ElementType { get { return _queryable.ElementType; } }

        Expression IQueryable.Expression { get { return _queryable.Expression; } }

        IQueryProvider IQueryable.Provider { get { return _queryable.Provider; } }
    }
}
