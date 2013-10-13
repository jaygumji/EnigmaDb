using System.Net.Configuration;
using Enigma.Modelling;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Enigma.Db.Engine;

namespace Enigma.Db
{
    public class EnigmaContext : IDisposable
    {

        private static readonly ContextReflectionManager ReflectionManager = new ContextReflectionManager();
        private readonly ContextReflectionDetails _details;

        private readonly IEnigmaConnection _engineConnection;
        private readonly bool _ownsConnection;

        private readonly IChangeManager _changeManager;
        private IEnigmaEngine _engine;



        public EnigmaContext(IEnigmaConnection engineConnection) : this(engineConnection, false)
        {
        }

        public EnigmaContext(IEnigmaConnection engineConnection, bool ownsConnection)
        {
            _engineConnection = engineConnection;
            _ownsConnection = ownsConnection;

            _details = ReflectionManager.GetDetails(this.GetType(), model =>
            {
                OnModelCreating(new ModelBuilder(model));
                _engine = _engineConnection.CreateEngine(model);
                _engine.Synchronize();
            });
            if (_engine == null)
                _engine = _engineConnection.CreateEngine(_details.Model);

            _changeManager = new ChangeManager(_details.Model);
            InitializeSets();
        }

        public Model Model { get { return _engine.Model; }}
        public IEnigmaConnection Connection { get { return _engineConnection; } }

        private void InitializeSets()
        {
            var enigmaSetType = typeof(EnigmaSet<>);
            foreach (var setProperty in _details.SetProperties)
            {
                var entityType = setProperty.PropertyType.GetGenericArguments()[0];
                var setInstance = Activator.CreateInstance(enigmaSetType.MakeGenericType(entityType), _engine, _changeManager);
                setProperty.SetValue(this, setInstance);
            }
        }

        protected virtual void OnModelCreating(ModelBuilder builder)
        {
        }

        public int SaveChanges()
        {
            return _changeManager.SaveChanges(_engine);
        }

        public void Dispose()
        {
            if (_ownsConnection)
                _engineConnection.Dispose();
        }

    }
}
