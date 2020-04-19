using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Planet.MongoDbConsoleAppSample.Enums;
using Planet.MongoDbConsoleAppSample.Models;

namespace Planet.MongoDbConsoleAppSample.SeedWork {
    public interface IUnitOfWork {
        Task SaveAsync<TEntity> (TEntity entity, CancellationToken cancellationToken = default) where TEntity : Entity; //For Mongo
        Task SaveAllAsync<TEntity> (IEnumerable<TEntity> entities, RecordOption recordOption, CancellationToken cancellationToken = default) where TEntity : Entity; //For Mongo
        Task SaveAllAsync<TEntity> (IEnumerable<TEntity> entities, InsertManyOptions insertManyOptions = null, CancellationToken cancellationToken = default) where TEntity : Entity; //For Mongo
        Task<int> CountAsync<TEntity> (AggregateOptions options = null,
            Expression<Func<TEntity, bool>> prediction = null, CancellationToken cancellationToken = default)
        where TEntity : Entity;

        Task<bool> AnyAsync<TEntity> (AggregateOptions options = null, Expression<Func<TEntity, bool>> prediction = null,
            CancellationToken cancellationToken = default) where TEntity : Entity;
    }
}