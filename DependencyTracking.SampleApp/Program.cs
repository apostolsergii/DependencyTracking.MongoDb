using System;
using DependencyTracking.Abstraction;
using DependencyTracking.Http;
using DependencyTracking.MongoDb;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DependencyTracking.SampleApp
{
    class Program
    {
        static void Main()
        {            
            var tracker = new DependencyTracker();
            
            MongoExample(tracker);
            
            HttpExample(tracker);

            Console.ReadKey();
        }

        private static void MongoExample(DependencyTracker tracker)
        {
            var connectionString = "mongodb://localhost:27017";
            var mongoClientFactory = new MongoClientFactory(tracker, null);
            var client = mongoClientFactory.GetClient(connectionString);
            var db = client.GetDatabase("TestDb");
            var collection = db.GetCollection<BsonDocument>("CollectionA");
            for (int i = 0; i < 5; i++)
            {
                collection.FindAsync(new JsonFilterDefinition<BsonDocument>("{'A':123}")).GetAwaiter().GetResult();
            }
        }

        private static void HttpExample(DependencyTracker tracker)
        {
            var targetUrl = "https://google.com.ua";
            var factory = new HttpClientFactory(tracker);
            var client = factory.GetClient();
            for (int i = 0; i < 5; i++)
            {
                client.GetAsync(targetUrl).GetAwaiter().GetResult();
            }            
        }
    }

    public class DependencyTracker : IDependencyTracker
    {
        public void Exception(Exception exception)
        {
            Console.WriteLine(exception);
        }

        public void Dependency(string type, string name, string description, bool success, TimeSpan duration)
        {
            var status = success ? "sucseed" : "failed";
            Console.WriteLine($"{type}: {name} ({description} {status} after {duration:g})");
        }

        public DependecyItem StartDependency(string type, string name, string description)
        {
            return new DependecyItem(this, type, name, description);
        }
    }
}
