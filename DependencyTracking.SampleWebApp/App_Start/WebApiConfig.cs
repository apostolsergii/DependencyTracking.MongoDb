using Autofac;
using Autofac.Integration.WebApi;
using DependencyTracking.Abstraction;
using DependencyTracking.ApplicationInsight;
using DependencyTracking.Http;
using DependencyTracking.MongoDb;
using Microsoft.ApplicationInsights;
using System.Reflection;
using System.Web.Http;

namespace DependencyTracking.SampleWebApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);

            builder.RegisterInstance(new DependencyLoggingSettings()).SingleInstance();
            builder.RegisterType<DependencyTracker>().As<IDependencyTracker>().SingleInstance();
            builder.RegisterType<MongoClientFactory>().SingleInstance();
            builder.RegisterType<HttpClientFactory>().SingleInstance();
            builder.RegisterType<TelemetryClient>().SingleInstance();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(builder.Build());


            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
