using System;
using DependencyTracking.Abstraction;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;

namespace DependencyTracking.ApplicationInsight
{    
    public class DependencyTracker : IDependencyTracker
    {
        public DependencyLoggingSettings Settings { get; } = 
                new DependencyLoggingSettings
                {
                    SlowDependencyDurationMs = 50,
                    Detailed = DetailsLevel.Slow
                };

        private readonly TelemetryClient _logger;

        public DependencyTracker(TelemetryClient logger, DependencyLoggingSettings settings)
        {
            _logger = logger;
            if (settings != null)
                Settings = settings;
        }

        public DependecyItem StartDependency(string type, string name, string description = null)
        {
            return new DependecyItem(this, type, name, description);
        }

        public void Dependency(string type, string name, string description, bool success, TimeSpan duration)
        {
            var dependency = new DependencyTelemetry
            {
                Duration = duration,
                Name = name,
                Success = success,
                Type = type
            };
            if (Settings.Detailed == DetailsLevel.All ||
                ((Settings.Detailed & DetailsLevel.Failed) == DetailsLevel.Failed && !success) ||
                ((Settings.Detailed & DetailsLevel.Slow) == DetailsLevel.Slow &&
                 duration.TotalMilliseconds >= Settings.SlowDependencyDurationMs))
            {
                dependency.Data = description;
            }
            else
            {
                if (!string.IsNullOrEmpty(description))
                    dependency.Data = description.Substring(0, Math.Min(50, description.Length - 1));
            }

            _logger.TrackDependency(dependency);
        }
    }
}
