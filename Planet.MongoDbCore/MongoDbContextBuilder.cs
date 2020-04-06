using System;
using System.Linq;
using System.Reflection;
using MongoDB.Driver;

namespace Planet.MongoDbCore {
    public class MongoDbContextBuilder<TService, TContext> : IMongoDbContextBuilder
    where TContext : MongoDbContext {
        int port = 27017;
        string host = "localhost";
        private Type dbContextType = typeof (TContext);
        public Type DbContextType {
            get {
                return dbContextType;
            }
            private set {
                dbContextType = value;
            }
        }
        public string DatabaseName { get; set; }
        public string Host {
            get {
                return host;
            }
            set {
                host = value;
            }
        }
        public int Port {
            get {
                return port;
            }
            set {
                port = value;
            }
        }
        public string Url { get; set; }

        public TService Build () {
            return Build (default (IServiceProvider));
        }

        public TService Build (IServiceProvider provider) {
            var dbContextName = DbContextType.FullName;
            var constructor = DbContextType
                .GetConstructors (BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault (p =>
                    p.GetParameters ()
                    .Count (x =>
                        x.ParameterType == typeof (MongoDbContextOptions)) > 0);
            var constructorParamsInfo = constructor.GetParameters ();
            var dbContextOptions = BuildOptions (constructorParamsInfo);

            var constratorParams = new object[constructorParamsInfo.Length];
            var i = 0;
            foreach (var p in constructorParamsInfo) {
                if (p.ParameterType == typeof (MongoDbContextOptions))
                    constratorParams[i] = dbContextOptions;
                else if (provider != default (IServiceProvider))
                    constratorParams[i] = provider.GetService (p.ParameterType);
                else
                    throw new ArgumentNullException ("You should pass a provider or database context options");
                i++;
            }

            return (TService) constructor.Invoke (constratorParams);
        }

        private MongoDbContextOptions BuildOptions (ParameterInfo[] constructorParamsInfo) {
            if (DatabaseName == null)
                throw new InvalidOperationException ($"Not set {nameof(DatabaseName)} value.");

            var options = new MongoDbContextOptions {
                DatabaseName = DatabaseName
            };

            if (!string.IsNullOrEmpty (this.Url))
                options.Url = new MongoUrlBuilder (Url) {
                    DatabaseName = DatabaseName
                }.ToMongoUrl ();
            else
                options.Settings = new MongoClientSettings {
                    Server = new MongoServerAddress (Host, Port)
                };

            return options;
        }
    }
}