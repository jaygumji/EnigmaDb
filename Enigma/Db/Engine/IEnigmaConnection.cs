using Enigma.Modelling;
using System;

namespace Enigma.Db
{
    public interface IEnigmaConnection : IDisposable
    {
        IEnigmaEngine CreateEngine(Model model);
    }
}
