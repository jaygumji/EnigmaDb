using System;
using System.Collections.Generic;
namespace Enigma.ProofOfConcept.Entities
{
    public class Article
    {

        public Article()
        {
            Id = Guid.NewGuid();
            CategoryIds = new List<Guid>();
        }

        public Guid Id { get; set; }
        public Guid AuthorId { get; set; }
        public List<Guid> CategoryIds { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }
        public string Tags { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
