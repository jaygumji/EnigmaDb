using Enigma.Db;
using Enigma.Db.Embedded;
using Enigma.Entities;
using Enigma.Modelling;
using Enigma.ProofOfConcept.Entities;
using System;
using System.IO;
namespace Enigma.ProofOfConcept.Context
{
    public class CommunityContext : EnigmaContext
    {

        public static readonly EmbeddedEnigmaService Service;

        static CommunityContext()
        {
            var directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CommunityDb");
            if (Directory.Exists(directory)) Directory.Delete(directory, true);

            Service = EmbeddedEnigmaService.CreateFileSystem(directory, new Store.CompositeStorageConfiguration { FragmentSize = DataSize.FromKB(5) });
        }

        public CommunityContext()
            : base(new EmbeddedEnigmaConnection(Service))
        {
        }

        public ISet<Article> Articles { get; set; }
        public ISet<Category> Categories { get; set; }
        public ISet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Article>()
                .Index(a => a.CreatedAt);
        }

    }
}
