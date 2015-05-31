using Enigma.Db;
using Enigma.Db.Embedded;
using Enigma.Entities;
using System;
using System.IO;
using Enigma.Testing.Fakes.Entities.Cars;

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
            _service.BackgroundQueue.WaitUntilIdle();
        }

        protected override void OnModelCreating(Enigma.Modelling.ModelBuilder builder)
        {
            builder.Entity<Car>()
                .Key(c => c.RegistrationNumber)
                .Index(c => c.Nationality)
                .Index(c => c.EstimatedValue)
                .Index(c => c.EstimatedAt)
                .Index(c => c.Engine.HorsePower);
        }

        public ISet<Car> Cars { get; set; }

    }
}
