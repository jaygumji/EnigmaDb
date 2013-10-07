EnigmaDb
========

EnigmaDb is a document database engine for .NET used to store entire entities in an easy way.
It's designed primary as an embedded service.

It relies on the following thirdparty open source libraries
- ProtoBuf.NET
- Remotion.Linq

Right now it's only tested on Windows. In the future the following is planned to be tested and supported.
- Windows
- Linux, Mono
- Windows Phone
- Android, Monodroid
- iOS, Monotouch

What's working right now?
- The storage of data
- Truncating data with a method call
- Indexes on simple properties on top level node of the entities
- Data retrieval through LINQ
- Inmemory storage, great for unittests

What's on the roadmap?
- Indexes that works with all type of properties
- Index rebuilding
- Ability to add jobs that runs at a scheduled time
- Maintainance job that truncates database at night and rebuilds indexes
- Better lock management to improve write performance


Example
To start with EnigmaDb you need to first create a context. This is done by creating a class that inherits EnigmaContext.

In the example below we're storing the database in the CommunityDb folder just under the application root directory.

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

    }

We have only one entity here, Article.
By default EnigmaDb looks for properties with the name Id, ID, Guid or GUID as the primary key

    public class Article
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Tags { get; set; }
        public DateTime CreatedAt { get; set; }
    }

All entities in EnigmaDb are POCO entities, they have no connection to the database.

Now to start using the database, we simply create a new instance of the context and start using it.

    public static class Program
    {
        public static void Main(string[] args)
        {
            // First we create a new instance of the context which
            // provides access to the database
            using (var context = new CommunityContext()) {
                // If we want to get all articles
                var allArticles = context.Articles.ToList();
                
                // If we want to get a specific article with the id of 11
                // this would retrieve the article with id 11
                var article = context.Articles.Get(11);
                
                // If we want to get all articles that was created today
                var date = DateTime.Now.Date;
                var articles = (from a in context.Articles
                                where a.CreatedAt > date
                                select a).ToList();
                
                // If we want to add a new article
                article = new Article {
                    Id = 100,
                    Subject = "EnigmaDb Example",
                    Body = "This is an example of the EnigmaDb document database",
                    Tags = "enigma db document database example",
                    CreatedAt = DateTime.Now,
                };
                context.Articles.Add(article);
                var count = context.SaveChanges();
                // count is the number of entities that was affected
                
                // If we want to update an entity with the id of 1
                if (context.Articles.TryGet(1, out article) {
                    article.Subject = "My changed subject";
                    context.SaveChanges();
                }
                
                // If we want to remove an entity with the id of 20
                article = context.Articles.Get(20);
                context.Articles.Remove(article);
                context.SaveChanges();
            }
        }
    }
