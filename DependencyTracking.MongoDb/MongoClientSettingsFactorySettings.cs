using System.Collections.Generic;

namespace DependencyTracking.MongoDb
{
    public class MongoClientSettingsFactorySettings
    {
        public IEnumerable<string> NotTrackedCommands { get; set; } = new[]
            {"isMaster", "buildInfo", "getLastError", "saslStart", "saslContinue"};
    }
}