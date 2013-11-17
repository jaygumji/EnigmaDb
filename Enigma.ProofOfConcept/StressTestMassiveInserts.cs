using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Enigma.ProofOfConcept.Context;

namespace Enigma.ProofOfConcept
{
    public class StressTestMassiveInserts : IConsoleCommand
    {
        public void Invoke()
        {
            ContextStressTest.SetupBasicCategoriesAndUsers();

            var startedAt = DateTime.Now;
            var tasks = new List<Task>();
            for (var i = 0; i < 5000; i++) {
                var task = Task.Factory.StartNew(() => {
                    using (var context = new CommunityContext()) {
                        context.Articles.Add(ContextStressTest.CreateUniqueArticle());
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

            Console.WriteLine("All tasks completed in {0}", allTasksCompletedAt.Subtract(startedAt));
            Console.WriteLine("from all tasks created {0}", allTasksCompletedAt.Subtract(tasksCreatedAt));

            var beganWriting5000PostsOn1Thread = DateTime.Now;
            Console.WriteLine("Preparing to write 5000 posts on 1 thread");
            using (var context = new CommunityContext()) {
                for (var i = 0; i < 5000; i++)
                    context.Articles.Add(ContextStressTest.CreateUniqueArticle());

                var count = context.SaveChanges();

                Console.WriteLine("Saved {0} additional articles in the database, time taken: {1}", count, DateTime.Now.Subtract(beganWriting5000PostsOn1Thread));
            }

            using (var context = new CommunityContext()) {
                var articles = context.Articles.Take(50).ToList().Concat(context.Articles.Skip(9950).Take(50)).ToList();
                foreach (var article in articles)
                    context.Articles.Remove(article);

                context.SaveChanges();
                Console.WriteLine(articles.Count + " articles was removed");
            }

            CommunityContext.Service.Truncate();

            Console.WriteLine("Database was truncated");

            using (var context = new CommunityContext()) {
                var articles = context.Articles.Take(10);
                foreach (var article in articles) {
                    Console.WriteLine("Article with subject {0} was retrieved", article.Subject);
                }
            }

        }
    }
}