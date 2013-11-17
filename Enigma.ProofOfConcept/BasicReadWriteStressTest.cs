using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Enigma.ProofOfConcept
{
    public static class BasicReadWriteStressTest
    {

        public static void Run()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var path = Path.Combine(baseDirectory, "Test.db");

            var buffer = new byte[102400];
            var random = new Random();
            random.NextBytes(buffer);

            using (var writeStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                writeStream.Write(buffer, 0, buffer.Length);
                writeStream.Write(BitConverter.GetBytes(43), 0, 4);
                writeStream.Flush();
                var tasks = new List<Task>();

                Console.WriteLine("Write stream was created, initializing writing");

                var failedLock = new object();
                int failedCount = 0;
                var successLock = new object();
                int successCount = 0;


                var writeTask = Task.Factory.StartNew(() =>
                {
                    for (var writeCount = 0; writeCount < 100; writeCount++)
                    {
                        System.Threading.Thread.Sleep(1);
                        writeStream.Write(buffer, 0, buffer.Length);
                        writeStream.Flush();
                    }

                    Console.WriteLine("Writing to write stream completed");
                });

                for (var i = 0; i < 20000; i++)
                {
                    var task = Task.Factory.StartNew(() =>
                    {
                        using (var readStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        {
                            readStream.Seek(102400, SeekOrigin.Begin);
                            var readBuffer = new byte[4];
                            readStream.Read(readBuffer, 0, 4);
                            var value = BitConverter.ToInt32(readBuffer, 0);
                            if (value == 43)
                                lock (successLock)
                                    successCount++;
                            else
                                lock (failedLock)
                                    failedCount++;
                        }
                    });
                    tasks.Add(task);
                }

                Console.WriteLine("{0} tasks was started", tasks.Count);

                Task.WaitAll(tasks.ToArray());

                Console.WriteLine("{0} was successful reads, {1} was unsuccessful reads", successCount, failedCount);

                foreach (var completedTask in tasks)
                    if (completedTask.Exception != null)
                        Console.WriteLine("Exception was thrown: {0}", completedTask.Exception.Message);

                writeTask.Wait();

                Console.ReadLine();
            }
        }

    }
}
