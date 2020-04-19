using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Planet.MongoDbConsoleAppSample.BsonMaps;
using Planet.MongoDbConsoleAppSample.Configurations;
using Planet.MongoDbConsoleAppSample.Enums;
using Planet.MongoDbConsoleAppSample.Models;
using Planet.MongoDbCore;
using Planet.MongoDbCore.Linq;

namespace Planet.MongoDbConsoleAppSample.Context {
    public class BlincatMongoDbContext : MongoDbContext, IBlincatMongoDbContext {
        private static readonly object InitializationLock = new object ();
        private readonly ILogger _logger;
        public BlincatMongoDbContext (string databaseName, ILogger logger, string host = "localhost", int port = 27017) : base (databaseName, host, port) {
            _logger = logger;
        }

        public BlincatMongoDbContext (string databaseName, ILogger logger, string url) : base (databaseName, url) {
            _logger = logger;
        }

        public BlincatMongoDbContext (ILogger logger, MongoDbContextOptions options) : base (options) {
            _logger = logger;
        }

        protected override async Task ConfigurateAsync () {
            lock (InitializationLock) {
                DatabaseBsonMap.Map ();
            }

            try {
                await IndexConfigurations<BlincatMongoDbContext>.CreateAllIndexes (this).ConfigureAwait (false);
            } catch (Exception ex) {
                _logger?.LogError (ex, $"Something wrong in ConfigurateAsync");
            }
        }

        public IMongoCollection<Bookmark> Bookmarks => GetCollection<Bookmark> ();
        public IMongoCollection<Image> Images => GetCollection<Image> ();

        public IMongoCollection<TEntity> GetCollection<TEntity> (CancellationToken cancellationToken = default) where TEntity : Entity {
            cancellationToken.ThrowIfCancellationRequested ();
            return base.GetCollection<TEntity> ();
        }

        public async Task SaveAsync<TEntity> (TEntity entity, CancellationToken cancellationToken = default) where TEntity : Entity {
            try {
                await GetCollection<TEntity> ().ReplaceOneAsync (r => r.Id.Equals (entity.Id), entity, new ReplaceOptions () { IsUpsert = true }, cancellationToken);
            } catch (TimeoutException ex) {
                _logger?.LogError ($"Timeout Exception in SaveAsync method. Source: {ex.Source}");
            } catch (MongoAuthenticationException ex) {
                _logger?.LogError ($"Mongo Authentication Exception in SaveAsync method. Source: {ex.Source}");
            } catch (Exception ex) {
                _logger?.LogError (ex, $"Entity Type: {typeof(TEntity)} | Entity ID:{entity.Id} - This record cannot be saved");
            }
        }

        /// <summary>
        /// Insert or update all entities on database
        /// </summary>
        /// <param name="entities">Entity list model</param>
        /// <param name="recordOption">Insert, update or upsert operation</param>
        /// <param name="cancellationToken">Token</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns></returns>
        public async Task SaveAllAsync<TEntity> (IEnumerable<TEntity> entities, RecordOption recordOption = RecordOption.Upsert, CancellationToken cancellationToken = default) where TEntity : Entity {
            try {
                var collection = GetCollection<TEntity> ();

                if (recordOption == RecordOption.Insert) {
                    await collection.InsertManyAsync (entities, new InsertManyOptions () { IsOrdered = true }, cancellationToken);
                }
                #region RecordOption.Update
                //else if (recordOption == RecordOption.Update)
                //{

                //        var updates = new List<WriteModel<TEntity>>();
                //        var filterBuilder = Builders<TEntity>.Filter;

                //        foreach (var doc in entities)
                //        {
                //            var filter = filterBuilder.Where(x => x.Id == doc.Id);
                //            updates.Add(new ReplaceOneModel<TEntity>(filter, doc));
                //        }

                //        await collection.BulkWriteAsync(updates, cancellationToken: cancellationToken);
                //} 
                #endregion
                else {
                    var bulkOps = entities
                        .Select (entity =>
                            new ReplaceOneModel<TEntity> (Builders<TEntity>.Filter.Where (x => x.Id == entity.Id), entity) { IsUpsert = true })
                        .Cast<WriteModel<TEntity>> ()
                        .ToList ();
                    await collection.BulkWriteAsync (bulkOps, cancellationToken : cancellationToken);
                }
            } catch (TimeoutException ex) {
                _logger?.LogError ($"Timeout Exception in SaveAsync method. Source: {ex.Source}");
            } catch (MongoAuthenticationException ex) {
                _logger?.LogError ($"Mongo Authentication Exception in SaveAsync method. Source: {ex.Source}");
            } catch (Exception ex) {
                _logger?.LogError (ex, $"These records cannot be saved");
            }
        }

        /// <summary>
        /// Insert all entities to database
        /// </summary>
        /// <param name="entities">Entity list model</param>
        /// <param name="insertManyOptions">To set insert options</param>
        /// <param name="cancellationToken">Token</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns></returns>
        public async Task SaveAllAsync<TEntity> (IEnumerable<TEntity> entities, InsertManyOptions insertManyOptions = null, CancellationToken cancellationToken = default) where TEntity : Entity {
            try {
                await GetCollection<TEntity> ().InsertManyAsync (entities, insertManyOptions, cancellationToken);
            } catch (TimeoutException ex) {
                _logger?.LogError ($"Timeout Exception in SaveAsync method. Source: {ex.Source}");
            } catch (MongoAuthenticationException ex) {
                _logger?.LogError ($"Mongo Authentication Exception in SaveAsync method. Source: {ex.Source}");
            } catch (Exception ex) {
                _logger?.LogError (ex, $"These records cannot be saved");
            }
        }

        public new IMongoQueryable<TEntity> AllQueryable<TEntity> (AggregateOptions options = null, CancellationToken cancellationToken = default) where TEntity : Entity {
            try {
                cancellationToken.ThrowIfCancellationRequested ();

                return base.AllQueryable<TEntity> (options, cancellationToken);
            } catch (TimeoutException ex) {
                _logger?.LogError ($"Timeout Exception in AllQueryable method. Source: {ex.Source}");
                return default;
            } catch (MongoAuthenticationException ex) {
                _logger?.LogError ($"Mongo Authentication Exception in AllQueryable method. Source: {ex.Source}");
                return default;
            } catch (Exception ex) {
                _logger?.LogError (ex, $"Something is wrong in AllQueryable");
                return new EmptyMongoQueryable<TEntity> ();
            }
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync<TEntity> (AggregateOptions options = null, CancellationToken cancellationToken = default) where TEntity : Entity {
            return await AllQueryable<TEntity> (options, cancellationToken).ToListAsync<TEntity> ();
        }

        public async Task<TEntity> GetAsync<TEntity> (string id, CancellationToken cancellationToken = default) where TEntity : Entity {
            var collection = GetCollection<TEntity> ();
            var filter = Builders<TEntity>.Filter.Eq ("Id", id);
            return await collection.Find (filter).FirstOrDefaultAsync (cancellationToken);
        }

        //https://stackoverflow.com/questions/31143586/mongodb-c-sharp-select-specific-field
        public async Task<TValue> GetFieldValue<TEntity, TValue> (string id, Expression<Func<TEntity, TValue>> fieldExpression, CancellationToken cancellationToken = default) where TEntity : Entity {
            var propertyValue = await GetCollection<TEntity> ()
                //.Find(_ => true)
                .Find (d => d.Id == id)
                .Project (new ProjectionDefinitionBuilder<TEntity> ().Expression (fieldExpression))
                .FirstOrDefaultAsync ();

            return propertyValue;
        }

        public async Task<TNewProjection> GetByProjection<TEntity, TNewProjection> (string id, Expression<Func<TEntity, TNewProjection>> fieldsExpression, CancellationToken cancellationToken = default) where TEntity : Entity {
            var projection = Builders<TEntity>.Projection.Expression (fieldsExpression);
            return await GetCollection<TEntity> ()
                .Find (d => d.Id == id)
                .Project<TNewProjection> (projection).FirstOrDefaultAsync ();
        }

        public async Task<TEntity> GetByExcludedField<TEntity> (string id, Expression<Func<TEntity, object>> fieldExpression, CancellationToken cancellationToken = default) where TEntity : Entity {
            var projection = Builders<TEntity>.Projection.Exclude (fieldExpression);
            return await GetCollection<TEntity> ()
                .Find (d => d.Id == id)
                .Project<TEntity> (projection).FirstOrDefaultAsync ();
        }

        //TODO: Field update: https://stackoverflow.com/questions/4818964/how-do-you-update-multiple-field-using-update-set-in-mongodb-using-official-c-sh
        //Todo: Move to service
        public async Task<bool> SoftDeleteAsync<TEntity> (string id, CancellationToken cancellationToken = default) where TEntity : Entity {
            var update = Builders<TEntity>.Update.Set (r => r.IsDeleted, true);
            var filter = Builders<TEntity>.Filter.Eq ("Id", id);
            var result = await GetCollection<TEntity> ().UpdateOneAsync (filter, update, new UpdateOptions () { IsUpsert = true }, cancellationToken);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteAsync<TEntity> (string id, CancellationToken cancellationToken = default) where TEntity : Entity {
            var filter = Builders<TEntity>.Filter.Eq ("Id", id);
            var result = await GetCollection<TEntity> ().DeleteOneAsync (filter, cancellationToken);
            return result.DeletedCount > 0;
        }

        public async Task<bool> UpdateOneFieldAsync<TEntity, TItem> (string id, Expression<Func<TEntity, TItem>> item, TItem value, CancellationToken cancellationToken = default)
        where TEntity : Entity {
            var update = Builders<TEntity>.Update.Set (item, value);
            var filter = Builders<TEntity>.Filter.Eq ("Id", id);
            var result = await GetCollection<TEntity> ().UpdateOneAsync (filter, update, new UpdateOptions () { IsUpsert = true }, cancellationToken);
            return result.ModifiedCount > 0;
        }

        public async Task<bool> UpdateAnArrayAsync<TEntity, TItem> (string id, Expression<Func<TEntity, IEnumerable<TItem>>> item, TItem value, CancellationToken cancellationToken = default)
        where TEntity : Entity
        where TItem : class {

            var update = Builders<TEntity>.Update.Push (item, value);
            var filter = Builders<TEntity>.Filter.Eq ("Id", id);
            var result = await GetCollection<TEntity> ().FindOneAndUpdateAsync (filter, update, null, cancellationToken);
            return result != null;
        }

        public async Task<int> CountAsync<TEntity> (AggregateOptions options = null, Expression<Func<TEntity, bool>> prediction = null, CancellationToken cancellationToken = default) where TEntity : Entity {
            if (prediction == null)
                return await AllQueryable<TEntity> (options, cancellationToken).CountAsync (cancellationToken);
            return await AllQueryable<TEntity> (options, cancellationToken).CountAsync (prediction, cancellationToken);
        }

        public async Task<bool> AnyAsync<TEntity> (AggregateOptions options = null, Expression<Func<TEntity, bool>> prediction = null, CancellationToken cancellationToken = default) where TEntity : Entity {

            if (prediction == null) {
                return await AllQueryable<TEntity> (options, cancellationToken).AnyAsync (cancellationToken);
            }
            return await AllQueryable<TEntity> (options, cancellationToken).AnyAsync (prediction, cancellationToken);
        }

        public void Dispose () {
            throw new NotImplementedException ();
        }
    }
}