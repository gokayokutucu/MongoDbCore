using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Planet.MongoDbConsoleAppSample.Models;

namespace Planet.MongoDbConsoleAppSample.SeedWork {
    public interface IUnitOfWork {
        Task SaveAsync<TEntity> (TEntity entity, CancellationToken cancellationToken = default) where TEntity : Entity; //For Mongo
        Task SaveAllAsync<TEntity> (IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : Entity; //For Mongo
        Task SaveAllAsync<TEntity> (IEnumerable<TEntity> entities, InsertManyOptions insertManyOptions = null, CancellationToken cancellationToken = default) where TEntity : Entity; //For Mongo
        Task CommitEntitiesAsync<TEntity> (List<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : Entity; //For Mongo
    }
}