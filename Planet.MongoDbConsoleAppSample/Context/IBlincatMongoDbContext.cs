using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Planet.MongoDbConsoleAppSample.Models;

namespace Planet.MongoDbConsoleAppSample.Context {
    public interface IBlincatMongoDbContext {
        IMongoCollection<Bookmark> Bookmarks { get; }
        IMongoCollection<Image> Images { get; }
        IMongoCollection<TEntity> GetCollection<TEntity> (CancellationToken cancellationToken = default) where TEntity : Entity;
        IMongoQueryable<TEntity> AllQueryable<TEntity> (AggregateOptions options = null, CancellationToken cancellationToken = default) where TEntity : Entity;
        Task<IEnumerable<TEntity>> GetAllAsync<TEntity> (AggregateOptions options = null, CancellationToken cancellationToken = default) where TEntity : Entity;
        Task<TEntity> GetAsync<TEntity> (string id, CancellationToken cancellationToken = default) where TEntity : Entity;
        Task<bool> DeleteAsync<TEntity> (string id, CancellationToken cancellationToken = default) where TEntity : Entity;
        Task<bool> SoftDeleteAsync<TEntity> (string id, CancellationToken cancellationToken = default) where TEntity : Entity;
        Task<string> SaveAsync<TEntity> (TEntity entity, CancellationToken cancellationToken = default) where TEntity : Entity; //For Mongo
        Task SaveAllAsync<TEntity> (IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : Entity; //For Mongo
        Task SaveAllAsync<TEntity> (IEnumerable<TEntity> entities, InsertManyOptions insertManyOptions = null, CancellationToken cancellationToken = default) where TEntity : Entity; //For Mongo
        Task CommitEntitiesAsync<TEntity> (List<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : Entity; //For Mongo
    }
}