using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Planet.MongoDbConsoleAppSample.Models;
using Planet.MongoDbConsoleAppSample.SeedWork;

namespace Planet.MongoDbConsoleAppSample.Context {
    public interface IBlincatMongoDbContext : IUnitOfWork {
        IMongoCollection<Bookmark> Bookmarks { get; }
        IMongoCollection<Image> Images { get; }
        IMongoCollection<TEntity> GetCollection<TEntity> (CancellationToken cancellationToken = default) where TEntity : Entity;
        IMongoQueryable<TEntity> AllQueryable<TEntity> (AggregateOptions options = null, CancellationToken cancellationToken = default) where TEntity : Entity;
        Task<IEnumerable<TEntity>> GetAllAsync<TEntity> (AggregateOptions options = null, CancellationToken cancellationToken = default) where TEntity : Entity;
        Task<TEntity> GetAsync<TEntity> (string id, CancellationToken cancellationToken = default) where TEntity : Entity;
        Task<bool> DeleteAsync<TEntity> (string id, CancellationToken cancellationToken = default) where TEntity : Entity;
        Task<bool> SoftDeleteAsync<TEntity> (string id, CancellationToken cancellationToken = default) where TEntity : Entity;
    }
}