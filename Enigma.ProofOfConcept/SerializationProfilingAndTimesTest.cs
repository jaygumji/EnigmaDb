using System;
using System.Diagnostics;
using System.IO;
using Enigma.Serialization.PackedBinary;
using Enigma.Testing.Fakes.Entities;

namespace Enigma.ProofOfConcept
{
    public class SerializationProfilingAndTimesTest : IConsoleCommand
    {
        public void Invoke()
        {
            var graph = DataBlock.Filled();

            var watch = new Stopwatch();
            watch.Start();
            Console.WriteLine("Initializing enigma serialization test...");

            long length;
            var serializer = new PackedDataSerializer<DataBlock>();
            using (var stream = new MemoryStream()) {
                serializer.Serialize(stream, graph);
                length = stream.Length;
            }

            var initializationTime = watch.Elapsed;

            for (var i = 0; i < 10000; i++) {
                serializer.Serialize(new MemoryStream(), graph);
            }

            var serializationTime = watch.Elapsed.Subtract(initializationTime);

            Console.WriteLine("Enigma serialization test completed");
            Console.WriteLine("Size of data: " + length);
            Console.WriteLine("Initialization time: " + initializationTime);
            Console.WriteLine("Serialization time: " + serializationTime);
        }
    }
}