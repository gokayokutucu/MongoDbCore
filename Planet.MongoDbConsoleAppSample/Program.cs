using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Planet.MongoDbConsoleAppSample.Application.Bookmarks.Queries;
using Planet.MongoDbConsoleAppSample.AutoMapper;
using Planet.MongoDbConsoleAppSample.Context;
using Planet.MongoDbConsoleAppSample.Infrastructure;
using Planet.MongoDbConsoleAppSample.Repositories;
using Planet.MongoDbCore;

namespace Planet.MongoDbConsoleAppSample {
    class Program {
        async static Task Main (string[] args) {
            var host = new HostBuilder ()
                .ConfigureAppConfiguration ((hostContext, configApp) => {
                    configApp.AddJsonFile ("appsettings.json", optional : true);
                    configApp.AddJsonFile (
                        $"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json",
                        optional : true);
                    configApp.AddEnvironmentVariables ();
                    configApp.AddCommandLine (args);
                })
                .ConfigureServices ((hostContext, services) => {
                    // Add AutoMapper
                    services.AddAutoMapper (new Assembly[] { typeof (AutoMapperProfile).GetTypeInfo ().Assembly });

                    // Add MediatR
                    services.AddTransient (typeof (IPipelineBehavior<,>), typeof (RequestPreProcessorBehavior<,>));
                    services.AddTransient (typeof (IPipelineBehavior<,>), typeof (RequestPerformanceBehaviour<,>));
                    services.AddTransient (typeof (IPipelineBehavior<,>), typeof (RequestValidationBehavior<,>));
                    services.AddMediatR (typeof (GetBookmarkListQueryHandler).GetTypeInfo ().Assembly);

                    // Add MongoDbCore
                    var config = hostContext.Configuration.Get<MongoDbContextConfiguration> ();
                    services.AddSingleton<IBlincatMongoDbContext> (s =>
                        new BlincatMongoDbContext (
                            config.DatabaseName,
                            s.GetService<IMediator> ()));
                    // services.AddMongoDbCore<IBlincatMongoDbContext, BlincatMongoDbContext> (options => {
                    //     options.DatabaseName = "blincatmongo";
                    // });
                    services.AddScoped<IBookmarkRepository, BookmarkRepository> ();
                })
                .ConfigureLogging ((hostContext, configLogging) => {
                    configLogging.AddConsole ();
                    configLogging.AddDebug ();
                })
                .Build ();

            await host.RunAsync ();
        }
    }
}