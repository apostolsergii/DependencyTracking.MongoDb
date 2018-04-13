using System.Net.Http;
using DependencyTracking.Abstraction;

namespace DependencyTracking.Http
{
    public class HttpClientFactory
    {
        private readonly IDependencyTracker _dependencyTracker;

        private readonly string _dependencyName;

        private const string DefaultDependencyName = "Http";

        public HttpClientFactory(IDependencyTracker dependencyTracker, string dependencyName = null)
        {
            _dependencyTracker = dependencyTracker;
            _dependencyName = dependencyName ?? DefaultDependencyName;
        }

        public HttpClient GetClient()
        {
            return new HttpClient(GetHandler());
        }

        public TrackedHttpClientHandler GetHandler()
        {
            return new TrackedHttpClientHandler(_dependencyTracker, _dependencyName);
        }
    }
}
