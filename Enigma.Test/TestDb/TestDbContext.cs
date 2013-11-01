using Enigma.Db;
using Enigma.Db.Embedded;
using Enigma.Entities;
using System;
using System.IO;

namespace Enigma.Test.TestDb
{
    public class TestDbContext : EnigmaContext
    {
        private readonly EmbeddedEnigmaService _service;

        private static EmbeddedEnigmaService CreateService()
        {
            return EmbeddedEnigmaService.CreateMemory();
        }

        public TestDbContext() : this(CreateService())
        {
        }

        public TestDbContext(EmbeddedEnigmaService service) : base(new EmbeddedEnigmaConnection(service))
        {
            _service = service;
        }

        public void WaitForBackgroundQueue()
        {
            _service.BackgroundQueue.WaitUntilEmpty();
        }

        protected override void OnModelCreating(Modelling.ModelBuilder builder)
        {
            builder.Entity<Car>()
                .Key(c => c.RegistrationNumber)
                .Index(c => c.Nationality)
                .Index(c => c.EstimatedValue)
                .Index(c => c.EstimatedAt);
        }

        public ISet<Car> Cars { get; set; }

    }
}
