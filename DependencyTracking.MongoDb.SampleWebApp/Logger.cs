using System;
using Microsoft.ApplicationInsights;

namespace DependencyTracking.MongoDb.SampleWebApp
{
    /// <summary>
    /// Logger implementation as a wrapper around TelemetryClient
    /// </summary>
    public class Logger : ILogger, IDependencyTracker
    {
        private readonly TelemetryClient _telemetryClient;

        public Logger(TelemetryClient telemetryClient)
        {
            _telemetryClient = telemetryClient;
        }

        public void Exception(Exception exception)
        {
            _telemetryClient.TrackException(exception);
        }

        public void Dependency(string name, string description, bool success, TimeSpan duration)
        {
            _telemetryClient.TrackDependency("DB", "MongoDb", name, description, DateTimeOffset.UtcNow, duration, null,
                success);
        }
    }
}