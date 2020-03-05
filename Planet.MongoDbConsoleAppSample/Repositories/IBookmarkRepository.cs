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
    public interface IBookmarkRepository : IRepository<Bookmark> {
        IMongoQueryable<Bookmark> AllQueryable (AggregateOptions options = null, CancellationToken cancellationToken = default);
        Task<IEnumerable<Bookmark>> GetAllByField (string fieldName, string fieldValue, CancellationToken cancellationToken = default);
        Task<IEnumerable<Bookmark>> GetAllBetween (int startingFrom, int count);
        Task<int> Count (Expression<Func<Bookmark, bool>> prediction = null);
    }
}