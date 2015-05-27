using System;
using System.Diagnostics;
using Enigma.ProofOfConcept.Entities;

namespace Enigma.ProofOfConcept
{
    public class ProtoBufTimesTest : IConsoleCommand
    {
        public void Invoke()
        {
            var graph = BigGraph.Filled();

            var watch = new Stopwatch();
            watch.Start();
            Console.WriteLine("Initializing ProtoBuf.NET serialization test...");
            var protobuf = new ProtocolBuffer.ProtocolBufferBinaryConverter<BigGraph>();
            var length = protobuf.Convert(graph).Length;

            Console.WriteLine(watch.Elapsed + " >> Initialization completed, size of data: " + length);

            Console.WriteLine(watch.Elapsed + " >> Running 10000 times...");

            for (var i = 0; i < 10000; i++) {
                protobuf.Convert(graph);
            }

            Console.WriteLine(watch.Elapsed + " >> ProtoBuf.NET serialization test completed");
        }
    }
}