using Enigma.Db.Linq;
using Enigma.Modelling;
namespace Enigma.Db
{
    public interface IEnigmaEngine
    {
        Model Model { get; }

        void Synchronize();
        IEnigmaEntityEngine<T> GetEntityEngine<T>();
        IEnigmaExpressionTreeExecutor CreateExecutor();
    }
}
