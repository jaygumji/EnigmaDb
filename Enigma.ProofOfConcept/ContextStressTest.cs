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

        public static readonly Int32KeyGenerator KeyGenerator = new Int32KeyGenerator(3);
        public static readonly Guid CategoryTestId = Guid.NewGuid();
        public static readonly Guid CategoryDatabaseId = Guid.NewGuid();
        public static readonly Guid SirTestUserId = Guid.NewGuid();

        private static bool _isBasicCategoriesAndUsersInitialized;
        public static void SetupBasicCategoriesAndUsers()
        {
            if (_isBasicCategoriesAndUsersInitialized) return;
            _isBasicCategoriesAndUsersInitialized = true;

            using (var context = new CommunityContext()) {
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

                var article = CreateUniqueArticle();
                context.Articles.Add(article);
                context.SaveChanges();
            }
        }

        public static Article CreateUniqueArticle()
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

    }
}
