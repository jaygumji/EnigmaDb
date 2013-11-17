using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Enigma.ProofOfConcept.Context;

namespace Enigma.ProofOfConcept
{
    public class ResourceContentionProfiling : IConsoleCommand
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
        }
    }
}
