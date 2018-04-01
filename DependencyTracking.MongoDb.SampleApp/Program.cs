using System;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DependencyTracking.MongoDb.SampleApp
{
    class Program
    {
        static void Main()
        {
            var connectionString = "mongodb://localhost:27017";

            var logger = new Logger();
            var mongoClientFactory = new MongoClientFactory(logger, new MongoClientSettingsFactorySettings(), logger);
            var client = mongoClientFactory.GetClient(connectionString);
            var db = client.GetDatabase("TestDb");
            var collection = db.GetCollection<BsonDocument>("CollectionA");
            for (int i = 0; i < 5; i++)
            {
                collection.FindAsync(new JsonFilterDefinition<BsonDocument>("{'A':123}")).GetAwaiter().GetResult();
            }
            
            Console.ReadKey();
        }
    }

    public class Logger : ILogger, IDependencyTracker
    {
        public void Exception(Exception exception)
        {
            Console.WriteLine(exception);
        }

        public void Dependency(string name, string description, bool success, TimeSpan duration)
        {
            var status = success ? "sucseed" : "failed";
            Console.WriteLine($"MongoDb operation {name} ({description} {status} after {duration:g})");
        }
    }
}
