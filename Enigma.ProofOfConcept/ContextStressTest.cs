using Enigma.ProofOfConcept.Context;
using Enigma.ProofOfConcept.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Enigma.ProofOfConcept
{
    public static class ContextStressTest
    {

        private static readonly Int32KeyGenerator KeyGenerator = new Int32KeyGenerator(3);

        private static Article CreateUniqueArticle()
        {
            var id = KeyGenerator.Next();
            var article = new Article
            {
                Id = id,
                Subject = "Autogeneration, entry " + id,
                Body = "Stress test",
                Tags = "enigma db document database stress test",
                CreatedAt = new DateTime(new DateTime(2012, 12, 07, 10, 00, 00).Ticks + id),
                Categories = new List<Category> { new Category { Name = "Test" } },
                Author = new User
                {
                    FirstName = "Sir",
                    LastName = "Test",
                    Nick = "Testmaniac",
                    Email = "test@jaygumji.com",
                    Password = System.Guid.NewGuid().ToByteArray()
                }
            };
            return article;
        }

        public static void SimpleTest()
        {
            Article article;

            using (var context = new CommunityContext())
            {
                context.Articles.ToArray();
                if (!context.Articles.TryGet(1, out article))
                {
                    article = new Article
                    {
                        Id = 1,
                        Subject = "Enigma Db",
                        Body = "Welcome to EnigmaDb",
                        Tags = "enigma db document database",
                        CreatedAt = new DateTime(2012, 12, 07, 10, 00, 00),
                        Categories = new List<Category> { new Category { Name = "Databases" } },
                        Author = new User
                        {
                            FirstName = "Johan",
                            LastName = "Johnsson",
                            Nick = "JayGuMJi",
                            Email = "johan.johnsson@outlook.com"
                        }
                    };
                    context.Articles.Add(article);
                }

                article.Subject = "Changed to a much longer text that needs more space";

                if (!context.Articles.TryGet(2, out article))
                    context.Articles.Add(new Article
                    {
                        Id = 2,
                        Subject = "Enigma Db and LINQ",
                        Body = "It will work",
                        Tags = "enigma db document database linq",
                        CreatedAt = new DateTime(2013, 01, 27, 21, 00, 00),
                        Categories = new List<Category> { new Category { Name = "Databases" } },
                        Author = new User {
                            FirstName = "Martin",
                            LastName = "Van der Cal",
                            Nick = "Cal",
                            Email = "martin.vandercal@gmail.com"
                        }
                    });
                var count = context.SaveChanges();
                Console.WriteLine("Saved {0} changes", count);

                article = context.Articles.Get(1);
                Console.WriteLine("Article subject: " + article.Subject);
            }

            using (var context = new CommunityContext())
            {
                article = context.Articles.Get(2);
                Console.WriteLine("Article subject: " + article.Subject);
            }

            using (var context = new CommunityContext())
            {
                var q = (from a in context.Articles
                         where a.Author.Nick == "Cal" && a.CreatedAt > DateTime.Parse("2013-01-26")
                         select a);

                article = q.First();

                Console.WriteLine("Linq selected Id is " + article.Id);
            }

        }

        public static void MassiveInserts()
        {
            var startedAt = DateTime.Now;
            var tasks = new List<Task>();
            for (var i = 0; i < 10000; i++)
            {
                var task = Task.Factory.StartNew(() =>
                {
                    using (var context = new CommunityContext())
                    {
                        context.Articles.Add(CreateUniqueArticle());
                        var count = context.SaveChanges();
                        //Console.WriteLine("Saved {0} changes", count);
                    }
                });
                tasks.Add(task);
            }
            var tasksCreatedAt = DateTime.Now;

            Console.WriteLine("Task creation completed in {0}", tasksCreatedAt.Subtract(startedAt));

            Task.WaitAll(tasks.ToArray());
            var allTasksCompletedAt = DateTime.Now;

            Console.WriteLine("All tasks completed in {0}, from all tasks created {1}", allTasksCompletedAt.Subtract(startedAt), allTasksCompletedAt.Subtract(tasksCreatedAt));

            var beganWriting10000PostsOn1Thread = DateTime.Now;
            Console.WriteLine("Preparing to write 10000 posts on 1 thread");
            using (var context = new CommunityContext())
            {
                for (var i = 0; i < 10000; i++)
                    context.Articles.Add(CreateUniqueArticle());
                
                var count = context.SaveChanges();

                Console.WriteLine("Saved {0} additional articles in the database, time taken: {1}", count, DateTime.Now.Subtract(beganWriting10000PostsOn1Thread));
            }

            using (var context = new CommunityContext())
            {
                var articles = context.Articles.Take(50).ToList().Concat(context.Articles.Skip(9950).Take(50)).ToList();
                foreach (var article in articles)
                    context.Articles.Remove(article);

                context.SaveChanges();
                Console.WriteLine(articles.Count + " articles was removed");
            }

            CommunityContext.Service.Truncate();

            Console.WriteLine("Database was truncated");

            using (var context = new CommunityContext())
            {
                var articles = context.Articles.Take(10);
                foreach (var article in articles)
                {
                    Console.WriteLine("Article ({0}) with subject {1} was retrieved", article.Id, article.Subject);
                }
            }

        }

    }
}
