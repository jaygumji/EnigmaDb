using Enigma.Modelling;
namespace Enigma.Db.Embedded
{
    public class EmbeddedEnigmaConnection : IEnigmaConnection
    {

        private readonly EmbeddedEnigmaService _service;

        public EmbeddedEnigmaConnection(EmbeddedEnigmaService service)
        {
            _service = service;
        }

        public IEnigmaEngine CreateEngine(Model model)
        {
            return new EmbeddedEnigmaEngine(model, _service);
        }

        public void Dispose()
        {
        }

    }
}
