using System;
using DependencyTracking.Abstraction;
using MongoDB.Driver;

namespace DependencyTracking.MongoDb
{
    public class MongoClientFactory
    {
        private readonly MongoTrackingConfigurator _configurator;

        public MongoClientFactory(IDependencyTracker dependencyTracker,
            MongoClientSettingsFactorySettings settings = null,
            ILogger logger = null)
        {
            _configurator = new MongoTrackingConfigurator(dependencyTracker, settings, logger);
        }

        public MongoClientSettings Get(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException($"{nameof(connectionString)} should not be empty");

            var mongoUrl = MongoUrl.Create(connectionString);
            if (mongoUrl == null)
                throw new ArgumentException($"Incorrect format of {nameof(connectionString)} - {connectionString}");

            return Get(mongoUrl);
        }

        public MongoClientSettings Get(MongoUrl mongoUrl)
        {
            if (mongoUrl == null)
                throw new ArgumentException($"{nameof(mongoUrl)} shoould not be empty");

            var settings = MongoClientSettings.FromUrl(mongoUrl);
            _configurator.ConfigureTracking(settings);

            return settings;
        }

        public MongoClient GetClient(string connectionString)
        {
            return new MongoClient(Get(connectionString));
        }

        public MongoClient GetClient(MongoUrl mongoUrl)
        {
            return new MongoClient(Get(mongoUrl));
        }
    }
}