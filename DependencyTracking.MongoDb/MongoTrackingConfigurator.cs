using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Linq;
using DependencyTracking.Abstraction;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace DependencyTracking.MongoDb
{
    internal class MongoTrackingConfigurator
    {
        private readonly ConcurrentDictionary<int, string> _queriesBuffer = new ConcurrentDictionary<int, string>();

        private readonly IDependencyTracker _dependencyTracker;

        public MongoTrackingConfigurator(IDependencyTracker dependencyTracker,
            MongoClientSettingsFactorySettings settings = null,
            ILogger logger = null)
        {
            _dependencyTracker = dependencyTracker ?? throw new ArgumentNullException($"{nameof(dependencyTracker)} is required");
            var notTrackedCommands = (settings ?? new MongoClientSettingsFactorySettings()).NotTrackedCommands.Select(v=>v.ToLower()).ToImmutableHashSet();

            OnCommandStartEvent = e =>
            {
                try
                {
                    if (e.Command != null && !notTrackedCommands.Contains(e.CommandName.ToLower()))
                    {
                        // ReSharper disable once SpecifyACultureInStringConversionExplicitly
                        _queriesBuffer.TryAdd(e.RequestId, e.Command.ToString());
                    }
                }
                catch (Exception exception)
                {
                    logger?.Exception(exception);
                }
            };

            OnCommandSucceededEvent = e =>
            {
                if (notTrackedCommands.Contains(e.CommandName.ToLower()))
                    return;

                try
                {
                    if (_queriesBuffer.TryRemove(e.RequestId, out var query))
                    {
                        OnCommandCompleted(
                            new MongoCommandCompletedEventArgs(e.CommandName, query, true,
                                e.Duration));
                    }
                }
                catch (Exception exception)
                {
                    logger?.Exception(exception);
                }
            };

            OnCommandFailedEvent = e =>
            {
                if (notTrackedCommands.Contains(e.CommandName.ToLower()))
                    return;
                try
                {
                    if (_queriesBuffer.TryRemove(e.RequestId, out var query))
                        OnCommandCompleted(
                            new MongoCommandCompletedEventArgs(e.CommandName, query, false,
                                e.Duration));
                }
                catch (Exception exception)
                {
                    logger?.Exception(exception);
                }
            };
        }

        private void OnCommandCompleted(MongoCommandCompletedEventArgs args)
        {
            _dependencyTracker.Dependency(args.CommandName, args.Query, args.Success, args.Duration);
        }

        internal readonly Action<CommandStartedEvent> OnCommandStartEvent;

        internal readonly Action<CommandSucceededEvent> OnCommandSucceededEvent;

        internal readonly Action<CommandFailedEvent> OnCommandFailedEvent;

        public void ConfigureTracking(MongoClientSettings clientSettings)
        {
            clientSettings.ClusterConfigurator += cb =>
            {
                cb.Subscribe(OnCommandStartEvent);
                cb.Subscribe(OnCommandSucceededEvent);
                cb.Subscribe(OnCommandFailedEvent);
            };
        }

        private class MongoCommandCompletedEventArgs : EventArgs
        {
            public MongoCommandCompletedEventArgs(string commandName, string query, bool success, TimeSpan duration)
            {
                CommandName = commandName;
                Query = query;
                Success = success;
                Duration = duration;
            }

            public string CommandName { get; }

            public string Query { get; }

            public bool Success { get; }

            public TimeSpan Duration { get; }
        }
    }
}