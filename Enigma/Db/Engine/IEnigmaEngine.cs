using Enigma.Modelling;
namespace Enigma.Db
{
    public interface IEnigmaEngine
    {
        void Synchronize(Model model);
        IEnigmaEntityEngine<T> GetEntityEngine<T>();
    }
}
