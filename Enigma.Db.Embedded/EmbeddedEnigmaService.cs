using System;
using Enigma.Db.Engine;
using Enigma.Db.Linq;
using Enigma.Modelling;
using Enigma.Store;
using Enigma.Store.FileSystem;
using Enigma.Store.Memory;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Enigma.Threading;

namespace Enigma.Db.Embedded
{
    /// <summary>
    /// Class to embedd enigma into a process
    /// </summary>
    public class EmbeddedEnigmaService : IDisposable
    {

        private readonly ConcurrentDictionary<string, EntityTable> _tables;
        private readonly IStorageFactoryService _service;
        private readonly BackgroundQueue _backgroundQueue;
        private readonly EmbeddedEnigmaConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedEnigmaService"/> class.
        /// </summary>
        /// <param name="service">A service used to make enigma aware of how to store the data</param>
        public EmbeddedEnigmaService(IStorageFactoryService service)
        {
            _tables = new ConcurrentDictionary<string, EntityTable>();
            _service = service;
            _configuration = new EmbeddedEnigmaConfiguration();

            _backgroundQueue = new BackgroundQueue();
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>
        /// The configuration.
        /// </value>
        public EmbeddedEnigmaConfiguration Configuration { get { return _configuration; } }

        /// <summary>
        /// Gets the background queue.
        /// </summary>
        /// <value>
        /// The background queue.
        /// </value>
        public BackgroundQueue BackgroundQueue { get { return _backgroundQueue; } }

        /// <summary>
        /// Creates a service which stores data in the file system.
        /// </summary>
        /// <param name="baseDirectory">The base directory of the database.</param>
        /// <returns>Enigma service used to store data</returns>
        public static EmbeddedEnigmaService CreateFileSystem(string baseDirectory)
        {
            return new EmbeddedEnigmaService(new FileSystemStorageFactoryService(baseDirectory, new CompositeStorageConfiguration()));
        }

        /// <summary>
        /// Creates a service which stores data in the file system.
        /// </summary>
        /// <param name="baseDirectory">The base directory of the database.</param>
        /// <param name="configuration">The configuration.</param>
        /// <returns>Enigma service used to store data</returns>
        public static EmbeddedEnigmaService CreateFileSystem(string baseDirectory, CompositeStorageConfiguration configuration)
        {
            return new EmbeddedEnigmaService(new FileSystemStorageFactoryService(baseDirectory, configuration));
        }

        /// <summary>
        /// Creates a service which stores data in the memory.
        /// </summary>
        /// <returns></returns>
        public static EmbeddedEnigmaService CreateMemory()
        {
            return new EmbeddedEnigmaService(new MemoryStorageFactoryService());
        }

        internal EntityTable Table(string name)
        {
            return _tables.GetOrAdd(name, n => {
                var storage = _service.CreateStorage(name);
                var indexes = _service.CreateIndexes(name);
                return new EntityTable(storage, indexes);
            });
        }

        /// <summary>
        /// Truncates all data in this instance.
        /// </summary>
        /// <remarks><para>This can be an extensive operation.
        /// If your database is big it's recommended to do this when things are calm.</para></remarks>
        public void Truncate()
        {
            var tables = _tables.Values.ToList();
            foreach (var table in tables)
                table.Storage.Maintenance.Truncate();
        }

        /// <summary>
        /// Tries to add content.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="content">The content.</param>
        /// <returns><c>true</c> if it was successfully stored, otherwise <c>false</c></returns>
        public bool TryAdd(string name, IKey key, byte[] content)
        {
            return Table(name).Storage.TryAdd(key, content);
        }

        /// <summary>
        /// Tries to update content.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="content">The content.</param>
        /// <returns><c>true</c> if it was successfully stored, otherwise <c>false</c></returns>
        public bool TryUpdate(string name, IKey key, byte[] content)
        {
            return Table(name).Storage.TryUpdate(key, content);
        }

        /// <summary>
        /// Tries to remove content.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if it was successfully removed, otherwise <c>false</c></returns>
        public bool TryRemove(string name, IKey key)
        {
            return Table(name).Storage.TryRemove(key);
        }

        /// <summary>
        /// Tries to get content.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        /// <param name="key">The key.</param>
        /// <param name="content">The content.</param>
        /// <returns><c>true</c> if it was successfully retrieved, otherwise <c>false</c></returns>
        public bool TryGet(string name, IKey key, out byte[] content)
        {
            return Table(name).Storage.TryGet(key, out content);
        }

        /// <summary>
        /// Synchronizes the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void Synchronize(Model model)
        {
            _service.SynchronizeModel(model);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _backgroundQueue.Dispose();
        }
    }
}
