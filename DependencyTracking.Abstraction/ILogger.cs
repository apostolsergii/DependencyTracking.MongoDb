using System;

namespace DependencyTracking.Abstraction
{
    public interface ILogger
    {
        void Exception(Exception exception);
    }
}