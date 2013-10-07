using Enigma.Modelling;
using Enigma.ProtocolBuffer;
using System;
using System.Collections.Generic;

namespace Enigma.Db.Embedded
{
    internal class EmbeddedEnigmaEngine : IEnigmaEngine
    {
        private readonly Dictionary<Type, IEnigmaEntityEngine> _entityEngines;
        private readonly Model _model;
        private readonly EmbeddedEnigmaService _service;

        public EmbeddedEnigmaEngine(Model model, EmbeddedEnigmaService service)
        {
            _entityEngines = new Dictionary<Type, IEnigmaEntityEngine>();
            _model = model;
            _service = service;
        }

        public IEnigmaEntityEngine<T> GetEntityEngine<T>()
        {
            Type entityType = typeof(T);
            IEnigmaEntityEngine entityEngine;
            if (_entityEngines.TryGetValue(entityType, out entityEngine))
                return (IEnigmaEntityEngine<T>)entityEngine;

            var converter = new ProtocolBufferBinaryConverter<T>(_model);
            var newEntityEngine = new EmbeddedEnigmaEntityEngine<T>(_model.Entity<T>(), _service, converter);
            _entityEngines.Add(entityType, newEntityEngine);
            return newEntityEngine;
        }

        public void Synchronize(Model model)
        {
            _service.Synchronize(model);
        }
    }
}
