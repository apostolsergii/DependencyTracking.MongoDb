using System;

namespace DependencyTracking.Abstraction
{
    public interface IDependencyTracker
    {
        void Dependency(string name, string description, bool success, TimeSpan duration);
    }
}