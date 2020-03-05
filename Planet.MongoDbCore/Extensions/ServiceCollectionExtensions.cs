using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Planet.MongoDbCore;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddMongoDbCore<TService, TContext> (this IServiceCollection services, IConfiguration configuration)
    where TService : class
    where TContext : MongoDbContext {
        if (configuration == null)
            throw new ArgumentNullException (nameof (configuration));

        services.AddSingleton<TContext> ();
        return AddMongoDbCore<TService, TContext> (services, configuration, null);
    }

    public static IServiceCollection AddMongoDbCore<TService, TContext> (this IServiceCollection services, IConfiguration configuration, Action<IMongoDbContextBuilder> optionsAction)
    where TService : class
    where TContext : MongoDbContext {
        if (configuration == null)
            throw new ArgumentNullException (nameof (configuration));

        var config = configuration.Get<MongoDbContextConfiguration> ();

        services.AddMongoDbCore<TService, TContext> (options => {
            options.DatabaseName = config.DatabaseName;
            options.Host = config.Host;
            options.Port = config.Port;
            options.Url = config.Url;

            optionsAction?.Invoke (options);
        });

        return services;
    }

    public static IServiceCollection AddMongoDbCore<TService, TContext> (this IServiceCollection services, Action<IMongoDbContextBuilder> optionsAction)
    where TService : class
    where TContext : MongoDbContext {
        if (optionsAction == null)
            throw new ArgumentNullException (nameof (optionsAction));

        var optionsBuilder = new MongoDbContextBuilder<TService, TContext> ();

        optionsAction.Invoke (optionsBuilder);

        services.AddSingleton<TService> (optionsBuilder.Build);

        return services;
    }

    public static IServiceCollection AddMongoDbCoreExtension<TContext, TExtension> (this IServiceCollection services)
    where TContext : MongoDbContext, TExtension
    where TExtension : class {
        services.AddSingleton<TExtension> (s => s.GetService<TContext> ());
        return services;
    }
}