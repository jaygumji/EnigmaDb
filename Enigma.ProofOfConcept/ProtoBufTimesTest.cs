using System;
using System.Diagnostics;
using Enigma.Testing.Fakes.Entities;

namespace Enigma.ProofOfConcept
{
    public class ProtoBufTimesTest : IConsoleCommand
    {
        public void Invoke()
        {
            var graph = DataBlock.Filled();

            var watch = new Stopwatch();
            watch.Start();
            Console.WriteLine("Initializing ProtoBuf.NET serialization test...");
            var protobuf = new ProtocolBuffer.ProtocolBufferBinaryConverter<DataBlock>();
            var length = protobuf.Convert(graph).Length;

            var initializationTime = watch.Elapsed;

            for (var i = 0; i < 10000; i++) {
                protobuf.Convert(graph);
            }

            var serializationTime = watch.Elapsed.Subtract(initializationTime);

            Console.WriteLine("Enigma serialization test completed");
            Console.WriteLine("Size of data: " + length);
            Console.WriteLine("Initialization time: " + initializationTime);
            Console.WriteLine("Serialization time: " + serializationTime);
        }
    }
}