using System;

namespace DependencyTracking.MongoDb
{
    public interface IDependencyTracker
    {
        void Dependency(string name, string description, bool success, TimeSpan duration);
    }
}