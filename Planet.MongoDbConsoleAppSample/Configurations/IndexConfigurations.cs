using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Planet.MongoDbConsoleAppSample.Enums;
using Planet.MongoDbConsoleAppSample.Exceptions;
using Planet.MongoDbConsoleAppSample.Models;
using Planet.MongoDbCore;

namespace Planet.MongoDbConsoleAppSample.Configurations {
    public class IndexConfigurations<TContext>
        where TContext : MongoDbContext {

            public static async Task CreateAllIndexes (TContext context, ILogger<TContext> logger = default, CancellationToken cancellationToken = default) {
                await Task.Run (() => {
                    var dbContextType = typeof (TContext);

                    // (IQueryable)Activator.CreateInstance(typeof(Queryable<>).
                    //   MakeGenericType(elementType), new object[] { this, expression });

                    var propertyInfos0 = dbContextType
                        .GetProperties ();
                    try {
                        var methodInfo = typeof (IndexConfigurations<TContext>).GetMethod ("EntityIndexAsync");

                        foreach (var property in propertyInfos0) {
                            var genericType = property.PropertyType.GenericTypeArguments.FirstOrDefault ();
                            var mi = dbContextType.GetMethod ("GetCollection", Type.EmptyTypes);
                            var miConstructed = mi?.MakeGenericMethod (genericType);
                            var collection = miConstructed?.Invoke (context, null);

                            var methodInfoConstructed = methodInfo?.MakeGenericMethod (genericType);
                            methodInfoConstructed?.Invoke (null, new [] { collection, logger, cancellationToken });
                        }
                    } catch (Exception ex) {
                        throw new PlanetDomainException ("IndexConfigurations",
                            "Runtime error. Some types cannot be resolved", ex);
                    }
                }, cancellationToken);
            }

            public static async Task EntityIndexAsync<TEntity> (IMongoCollection<TEntity> collection, ILogger<TContext> logger = default, CancellationToken cancellationToken = default) where TEntity : Entity {
                await CreateIndexAsync (collection, (e => e.CreatedDate), null, IndexKeyType.Ascending, logger, cancellationToken).ConfigureAwait (false);
            }

            //https://stackoverflow.com/questions/35019313/checking-if-an-index-exists-in-mongodb
            static async Task CreateIndexAsync<TEntity> (IMongoCollection<TEntity> collection, Expression<Func<TEntity, object>> field,
                CreateOneIndexOptions createOneIndexOptions = null, IndexKeyType indexKeyType = IndexKeyType.Ascending, ILogger<TContext> logger = default, CancellationToken cancellationToken = default) where TEntity : Entity {
                if (field == null)
                    throw new ArgumentNullException ($"Field expression cannot be empty");

                CreateIndexModel<TEntity> indexModel;
                switch (indexKeyType) {
                    case IndexKeyType.Descending:
                        indexModel = new CreateIndexModel<TEntity> (Builders<TEntity>.IndexKeys.Descending (field));
                        break;
                    case IndexKeyType.Text:
                        indexModel = new CreateIndexModel<TEntity> (Builders<TEntity>.IndexKeys.Text (field));
                        break;
                    case IndexKeyType.Hashed:
                        indexModel = new CreateIndexModel<TEntity> (Builders<TEntity>.IndexKeys.Hashed (field));
                        break;
                    default:
                        indexModel = new CreateIndexModel<TEntity> (Builders<TEntity>.IndexKeys.Ascending (field));
                        break;
                }
                try {
                    await collection.Indexes
                        .CreateOneAsync (indexModel, createOneIndexOptions, cancellationToken)
                        .ConfigureAwait (false);
                } catch (MongoConnectionException mongoConnectionException) {
                    logger?.LogError (mongoConnectionException, "Passbridge MongoDb database connection error");
                } catch (TimeoutException timeoutException) {
                    logger?.LogError (timeoutException, "Timeout Exception in CreateIndexAsync method");
                } catch (Exception ex) {
                    logger?.LogError (ex, "Something is wrong is MongoDB");
                }
            }

            static async Task CreateCombinedIndex<TEntity> (IMongoCollection<TEntity> collection, CreateOneIndexOptions createOneIndexOptions = null,
                IndexKeyType indexKeyType = IndexKeyType.Ascending, ILogger<TContext> logger = default, CancellationToken cancellationToken = default, params IndexKeysDefinition<TEntity>[] keys) where TEntity : Entity {
                if (keys == null)
                    throw new ArgumentNullException ($"Field expression cannot be empty");

                var indexModel = new CreateIndexModel<TEntity> (Builders<TEntity>.IndexKeys.Combine (keys));

                await collection.Indexes.CreateOneAsync (indexModel, createOneIndexOptions, cancellationToken).ConfigureAwait (false);
            }
        }
}