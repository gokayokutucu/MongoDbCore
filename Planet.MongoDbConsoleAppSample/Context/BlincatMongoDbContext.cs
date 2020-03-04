using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Planet.MongoDbConsoleAppSample.BsonMaps;
using Planet.MongoDbConsoleAppSample.Models;
using Planet.MongoDbCore;

namespace Planet.MongoDbConsoleAppSample.Context {
    public class BlincatMongoDbContext : MongoDbContext, IBlincatMongoDbContext {
        private static readonly object _initializationLock = new object ();
        private IMediator _mediator;
        public BlincatMongoDbContext (string databaseName, IMediator mediator, string host = "localhost", int port = 27017) : base (databaseName, host, port) {
            _mediator = mediator ??
                throw new ArgumentNullException (nameof (mediator));
        }

        public BlincatMongoDbContext (string databaseName, IMediator mediator, string url) : base (databaseName, url) {
            _mediator = mediator ??
                throw new ArgumentNullException (nameof (mediator));
        }

        public BlincatMongoDbContext (MongoDbContextOptions options, IMediator mediator) : base (options) {
            _mediator = mediator ??
                throw new ArgumentNullException (nameof (mediator));
        }

        protected override async Task ConfigurateAsync () {
            lock (_initializationLock) {
                DatabaseBsonMap.Map ();
            }

            await IndexConfigurations.CreateAllIndexes (this).ConfigureAwait (false);
        }

        private MongoClient CreateClient (MongoClientOptions options) {
            if (options.Url != null)
                return new MongoClient (options.Url);
            return new MongoClient (options.Settings);
        }

        private IMongoCollection<Bookmark> _bookmarks;
        public IMongoCollection<Bookmark> Bookmarks {
            get {
                _bookmarks = GetCollection<Bookmark> ();
                return _bookmarks;
            }
            private set {
                _bookmarks = value;
            }
        }

        private IMongoCollection<Image> _images;
        public IMongoCollection<Image> Images {
            get {
                _images = GetCollection<Image> ();
                return _images;
            }
            private set {
                _images = value;
            }
        }

        public IMongoCollection<TEntity> GetCollection<TEntity> (CancellationToken cancellationToken = default) where TEntity : Entity {
            cancellationToken.ThrowIfCancellationRequested ();
            return GetCollection<TEntity> ();
        }

        public async Task<string> SaveAsync<TEntity> (TEntity entity, CancellationToken cancellationToken = default) where TEntity : Entity {
            try {
                await GetCollection<TEntity> ().ReplaceOneAsync (r => r.Id.Equals (entity.Id), entity, new UpdateOptions () { IsUpsert = true }, cancellationToken);
                return entity.Id;
            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Insert or update all entities on database
        /// </summary>
        /// <param name="entities">Entity list model</param>
        /// <param name="cancellationToken">Token</param>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns></returns>
        public async Task SaveAllAsync<TEntity> (IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : Entity {
            try {
                var collection = GetCollection<TEntity> ();
                foreach (var entity in entities) {
                    await collection.ReplaceOneAsync (r => r.Id.Equals (entity.Id), entity, new UpdateOptions () { IsUpsert = true }, cancellationToken);
                }
            } catch (Exception ex) {
                throw ex;
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
            } catch (Exception ex) {
                throw ex;
            }
        }

        public IMongoQueryable<TEntity> AllQueryable<TEntity> (AggregateOptions options = null, CancellationToken cancellationToken = default) where TEntity : Entity {
            cancellationToken.ThrowIfCancellationRequested ();
            return GetCollection<TEntity> ().AsQueryable (options);
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

        public Task<int> SaveChangesAsync (CancellationToken cancellationToken = default) {
            throw new NotImplementedException ();
        }

        public Task CommitEntitiesAsync<TEntity> (List<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : Entity {
            throw new NotImplementedException ();
        }

        public void Dispose () {
            throw new NotImplementedException ();
        }
    }
}