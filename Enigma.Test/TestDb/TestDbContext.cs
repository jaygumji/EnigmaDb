using Enigma.Db;
using Enigma.Db.Embedded;
using Enigma.Entities;
using System;
using System.IO;

namespace Enigma.Test.TestDb
{
    public class TestDbContext : EnigmaContext
    {

        private static EmbeddedEnigmaService CreateService()
        {
            return EmbeddedEnigmaService.CreateMemory();
        }

        public TestDbContext() : base(new EmbeddedEnigmaConnection(CreateService()))
        {
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
