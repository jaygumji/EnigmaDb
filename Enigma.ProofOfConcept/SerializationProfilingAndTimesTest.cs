using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Enigma.ProofOfConcept.Entities;
using Enigma.Serialization.PackedBinary;

namespace Enigma.ProofOfConcept
{
    public class SerializationProfilingAndTimesTest : IConsoleCommand
    {
        public void Invoke()
        {
            var graph = BigGraph.Filled();

            var watch = new Stopwatch();
            watch.Start();
            Console.WriteLine("Initializing enigma serialization test...");

            long length;
            var serializer = new PackedDataSerializer<BigGraph>();
            using (var stream = new MemoryStream()) {
                serializer.Serialize(stream, graph);
                length = stream.Length;
            }

            Console.WriteLine(watch.Elapsed + " >> Initialization completed, size of data: " + length);

            Console.WriteLine(watch.Elapsed + " >> Running 10000 times...");

            for (var i = 0; i < 10000; i++) {
                serializer.Serialize(new MemoryStream(), graph);
            }

            Console.WriteLine(watch.Elapsed + " >> Enigma serialization test completed");
        }
    }
}