using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DependencyTracking.Abstraction;

namespace DependencyTracking.Http
{
    public class TrackedHttpClientHandler : HttpClientHandler
    {
        private readonly IDependencyTracker _dependencyTracker;

        private readonly string _dependencyName;        

        public TrackedHttpClientHandler(IDependencyTracker dependencyTracker, string dependencyName)
        {
            _dependencyTracker = dependencyTracker;
            _dependencyName = dependencyName;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            using (var dependency = _dependencyTracker.StartDependency(_dependencyName, request.RequestUri.Host,
                request.RequestUri.ToString()))
            {
                var result =  await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                dependency.Success = result.IsSuccessStatusCode;

                return result;
            }            

        }
    }
}