using System;
using Enigma.ProofOfConcept.Context;

namespace Enigma.ProofOfConcept
{
    public class Program
    {

        public static void Main(string[] args)
        {
            ContextStressTest.MassiveInserts();
            CommunityContext.Service.Dispose();
            Console.ReadLine();
        }

    }
}
