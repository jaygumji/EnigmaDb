using System;
using Enigma.ProofOfConcept.Context;

namespace Enigma.ProofOfConcept
{
    public class InstrumentationProfiling : IConsoleCommand
    {
        public void Invoke()
        {
            ContextStressTest.SetupBasicCategoriesAndUsers();

            var beganWriting5000PostsOn1Thread = DateTime.Now;
            Console.WriteLine("Preparing to write 5000 posts on 1 thread");
            using (var context = new CommunityContext()) {
                for (var i = 0; i < 5000; i++)
                    context.Articles.Add(ContextStressTest.CreateUniqueArticle());

                var count = context.SaveChanges();

                Console.WriteLine("Saved {0} additional articles in the database, time taken: {1}", count, DateTime.Now.Subtract(beganWriting5000PostsOn1Thread));
            }
        }
    }
}