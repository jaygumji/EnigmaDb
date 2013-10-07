using Enigma.Binary;
using Enigma.IO;
using Enigma.Modelling;
using Enigma.ProtocolBuffer;
using Enigma.Store;
using Enigma.Store.Binary;
using Enigma.Store.FileSystem;
using Enigma.Store.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enigma.ProofOfConcept
{

    public static class SingleStorageStressTest
    {

        public class MyEntity
        {
            public int Id { get; set; }
            public string Message { get; set; }
            public double Value { get; set; }
            public Category Category { get; set; }

            public bool HasMessage { get { return !string.IsNullOrEmpty(Message); } }

            public bool IsValid()
            {
                return Id == 1
                    && Message == "Hello World"
                    && Value == 42.5
                    && Category != null
                    && Category.Id == 1
                    && Category.Name == "First scenario";
            }
        }

        public class Category
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public static void Run()
        {
            var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SingeStorage");
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            var storagePath = Path.Combine(directory, "MyEntity.dat");
            if (File.Exists(storagePath)) File.Delete(storagePath);

            using (var store = new DualBinaryStore(new FileSystemStreamProvider(storagePath), 0, 5 * 1024 * 1024))
            {
                var converter = new ProtocolBufferBinaryConverter<MyEntity>();
                var storage = new StorageFragment(store);

                var content = converter.Convert(new MyEntity { Id = 1, Message = "Hello World", Value = 42.5, Category = new Category { Id = 1, Name = "First scenario" } });
                var added = storage.TryAdd(new Int32Key(1), content);

                Console.WriteLine("Value was {0} added", added ? "successfully" : "unsuccessfully");

                if (storage.TryGet(new Int32Key(1), out content))
                {
                    var entity = converter.Convert(content);
                    Console.WriteLine("Retrieve success, valdiation {0}", entity.IsValid() ? "successful" : "unsuccessful");
                }
                else
                {
                    Console.WriteLine("Error, could not get the content");
                }
            }
        }

    }
}
