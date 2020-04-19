using System;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Planet.MongoDbCore.Extensions;
using Planet.MongoDbCore.Linq;

namespace Planet.MongoDbCore {
    public abstract class MongoDbContext {
        protected IMongoDatabase _database;
        public MongoDbContext (string databaseName, string host = "localhost", int port = 27017) {
            Initialize (databaseName,
                new MongoClientOptions () { Settings = new MongoClientSettings { Server = new MongoServerAddress (host, port) } });
        }

        public MongoDbContext (string databaseName, string url) {
            Initialize (databaseName,
                new MongoClientOptions () { Url = new MongoUrl (url) });
        }

        public MongoDbContext (MongoDbContextOptions options) {
            Initialize (options.DatabaseName,
                new MongoClientOptions () { Url = options.Url, Settings = options.Settings });
        }

        private async void Initialize (string databaseName, MongoClientOptions options) {
            if (string.IsNullOrEmpty (databaseName)) throw new ArgumentNullException ("Database", "Your connection string must contain a database name");
            if (_database != null) throw new InvalidOperationException ("Database connection is already initialized!");
            try {
                _database = CreateClient (options).GetDatabase (databaseName);
                await ConfigurateAsync ();
            } catch (Exception) {
                _database = null;
                throw;
            }
        }
        protected abstract Task ConfigurateAsync ();
        private MongoClient CreateClient (MongoClientOptions options) {
            if (options.Url != null)
                return new MongoClient (options.Url);
            return new MongoClient (options.Settings);
        }

        public IMongoCollection<TEntity> GetCollection<TEntity> () {
            return _database.GetCollection<TEntity> (typeof (TEntity).GetCollectionName ());
        }

        public IMongoQueryable<TEntity> AllQueryable<TEntity> (AggregateOptions options = null, CancellationToken cancellationToken = default) {
            try {
                cancellationToken.ThrowIfCancellationRequested ();

                var command = new BsonDocument { { "ping", 1 }
                };
                var t = Task.Run (
                    async () => {
                        var result = await _database.RunCommandAsync<BsonDocument> (command, cancellationToken : cancellationToken);
                    }, cancellationToken);
                t.Wait (cancellationToken);

                return GetCollection<TEntity> ().AsQueryable (options);
            } catch (TimeoutException ex) {
                throw ex;
            } catch (MongoAuthenticationException ex) {
                throw ex;
            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}