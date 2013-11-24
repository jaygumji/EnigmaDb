using Enigma.Db.Embedded.Linq;
using Enigma.Db.Linq;
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

        public Model Model { get { return _model; } }

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

        public IEnigmaExpressionTreeExecutor CreateExecutor()
        {
            return new EmbeddedEnigmaExpressionTreeExecutor(_service);
        }

        public void Synchronize()
        {
            _service.Synchronize(_model);
        }
    }
}
