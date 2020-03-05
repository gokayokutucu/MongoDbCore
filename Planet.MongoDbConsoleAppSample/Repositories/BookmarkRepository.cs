using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Planet.MongoDbConsoleAppSample.Context;
using Planet.MongoDbConsoleAppSample.Models;
using Planet.MongoDbConsoleAppSample.SeedWork;

namespace Planet.MongoDbConsoleAppSample.Repositories {
    public class BookmarkRepository : IBookmarkRepository {
        public IUnitOfWork UnitOfWork => _context;
        private readonly IBlincatMongoDbContext _context;
        private readonly IMediator _mediator;
        public BookmarkRepository (IBlincatMongoDbContext context, IMediator mediator) {
            _context = context ??
                throw new ArgumentNullException (nameof (context));
            _mediator = mediator ??
                throw new ArgumentNullException (nameof (mediator));
        }

        // public async Task<string> SaveAsync (Bookmark entity, CancellationToken cancellationToken = default) {
        //     return await _context.CommitEntitiesAsync<Bookmark> (entity, cancellationToken);
        // }

        // public async Task SaveAllAsync (IEnumerable<Bookmark> entities, CancellationToken cancellationToken = default) {
        //     await _context.SaveAllAsync<Bookmark> (entities, cancellationToken);
        // }

        public async Task<bool> DeleteAsync (string id, CancellationToken cancellationToken = default) {
            return await _context.DeleteAsync<Bookmark> (id, cancellationToken);
        }

        public async Task<Bookmark> GetAsync (string id, CancellationToken cancellationToken = default) {
            return await _context.GetAsync<Bookmark> (id);
        }

        public IMongoQueryable<Bookmark> AllQueryable (AggregateOptions options = null, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested ();
            return _context.GetCollection<Bookmark> ().AsQueryable (options);
        }

        public async Task<IEnumerable<Bookmark>> GetAllAsync (CancellationToken cancellationToken = default) {
            return await _context.GetAllAsync<Bookmark> (null, cancellationToken);
        }

        public async Task<IEnumerable<Bookmark>> GetAllByField (string fieldName, string fieldValue, CancellationToken cancellationToken = default) {
            var filter = Builders<Bookmark>.Filter.Eq (fieldName, fieldValue);
            var result = await _context.GetCollection<Bookmark> ().Find (filter).ToListAsync ();

            return result;
        }

        public async Task<IEnumerable<Bookmark>> GetAllBetween (int startingFrom, int count) {
            var result = await _context.AllQueryable<Bookmark> ()
                .OrderBy (m => m.Id)
                .Skip (startingFrom)
                .Take (count)
                .ToListAsync ();

            return result;
        }

        public async Task<int> Count (Expression<Func<Bookmark, bool>> prediction = null) {
            if (prediction == null)
                return await _context.AllQueryable<Bookmark> ().CountAsync ();
            return await _context.AllQueryable<Bookmark> ().CountAsync (prediction);
        }
    }
}