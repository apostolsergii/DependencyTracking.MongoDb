using System;

namespace DependencyTracking.MongoDb
{
    public interface ILogger
    {
        void Exception(Exception exception);
    }
}