using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Planet.MongoDbConsoleAppSample.Models;
using Planet.MongoDbConsoleAppSample.SeedWork;

namespace Planet.MongoDbConsoleAppSample.Repositories {
    public interface IImageRepository : IRepository<Image> {
        IMongoQueryable<Image> AllQueryable (AggregateOptions options = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Image>> GetAllByFieldAsync (string fieldName, string fieldValue, CancellationToken cancellationToken = default);
        Task<IEnumerable<Image>> GetAllBetweenAsync (int startingFrom, int count);
        Task<int> Count (Expression<Func<Image, bool>> prediction = null);
    }
}