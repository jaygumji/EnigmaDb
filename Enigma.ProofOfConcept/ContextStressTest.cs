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
        private static readonly Guid CategoryTestId = Guid.NewGuid();
        private static readonly Guid CategoryDatabaseId = Guid.NewGuid();
        private static readonly Guid SirTestUserId = Guid.NewGuid();

        private static void SetupBasicCategoriesAndUsers(CommunityContext context)
        {
            context.Categories.Add(new Category {
                Id = CategoryTestId,
                Name = "Test"
            });
            context.Categories.Add(new Category {
                Id = CategoryDatabaseId,
                Name = "Database"
            });

            context.Users.Add(new User {
                Id = SirTestUserId,
                FirstName = "Sir",
                LastName = "Test",
                Nick = "Testmaniac",
                Email = "test@jaygumji.com",
                Password = System.Guid.NewGuid().ToByteArray()
            });
        }

        private static Article CreateUniqueArticle()
        {
            var id = KeyGenerator.Next();
            var article = new Article
            {
                Subject = "Autogeneration, entry " + id,
                Body = "Stress test",
                Tags = "enigma db document database stress test",
                CreatedAt = new DateTime(new DateTime(2012, 12, 07, 10, 00, 00).Ticks + id),
                CategoryIds = {CategoryTestId},
                AuthorId = SirTestUserId
            };
            return article;
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
