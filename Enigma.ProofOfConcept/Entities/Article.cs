using System;
using System.Collections.Generic;
namespace Enigma.ProofOfConcept.Entities
{
    public class Article
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Tags { get; set; }
        public List<Category> Categories { get; set; }
        public User Author { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
