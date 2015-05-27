using System;
using System.Collections.Generic;

namespace Enigma.ProofOfConcept.Entities
{
    public class BigGraph
    {

        public Guid Id { get; set; }
        public User User { get; set; }
        public Dictionary<Guid, Article> Articles { get; set; }
        public List<Category> Categories { get; set; }
        //public int[,] TheMatrix { get; set; }

        public static BigGraph Filled()
        {
            return new BigGraph {
                Id = Guid.NewGuid(),
                User = new User {
                    Id = Guid.NewGuid(),
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "john.doe@testcases.com",
                    Nick = "Johnny",
                    Password = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 }
                },
                Articles = new Dictionary<Guid, Article> {
                    {Guid.NewGuid(), new Article {
                        Id = Guid.NewGuid(),
                        AuthorId = Guid.NewGuid(),
                        Body = "Hello World",
                        CategoryIds = new List<Guid> {Guid.NewGuid(), Guid.NewGuid()},
                        CreatedAt = DateTime.Now,
                        Subject = "Greeting",
                        Tags = "Greeting Hello World"
                    }}
                },
                Categories = new List<Category> {
                    new Category {Id = Guid.NewGuid(), Name = "One"},
                    new Category {Id = Guid.NewGuid(), Name = "Two"},
                    new Category {Id = Guid.NewGuid(), Name = "Three"}
                },
                //TheMatrix = new[,] { { 1, 2 }, { 3, 4} }
            };
        }

    }
}
