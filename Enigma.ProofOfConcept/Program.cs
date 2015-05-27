using Enigma.ProofOfConcept.Context;

namespace Enigma.ProofOfConcept
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var ui = new ConsoleUI();
            ui.AddCommand(new StressTestMassiveInserts());
            ui.AddCommand(new InstrumentationProfiling());
            ui.AddCommand(new ResourceContentionProfiling());
            ui.AddCommand(new SerializationProfilingAndTimesTest());
            ui.AddCommand(new ProtoBufTimesTest());
            ui.Run();
        }

    }
}
