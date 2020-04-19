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
    public class ImageRepository : IImageRepository {
        public IUnitOfWork UnitOfWork => _context;
        private readonly IBlincatMongoDbContext _context;
        private readonly IMediator _mediator;
        public ImageRepository (IBlincatMongoDbContext context, IMediator mediator) {
            _context = context ??
                throw new ArgumentNullException (nameof (context));
            _mediator = mediator ??
                throw new ArgumentNullException (nameof (mediator));
        }
        public IMongoQueryable<Image> AllQueryable (AggregateOptions options = null, CancellationToken cancellationToken = default) {
            cancellationToken.ThrowIfCancellationRequested ();
            return _context.AllQueryable<Image> (options, cancellationToken);
        }

        public Task<int> Count (Expression<Func<Image, bool>> prediction = null) {
            throw new NotImplementedException ();
        }

        public Task<bool> DeleteAsync (string id, CancellationToken cancellationToken = default) {
            throw new NotImplementedException ();
        }

        public async Task<IEnumerable<Image>> GetAllAsync (CancellationToken cancellationToken = default) {
            return await _context.GetAllAsync<Image> (null, cancellationToken);
        }

        public async Task<IEnumerable<Image>> GetAllBetweenAsync (int startingFrom, int count) {
            var result = await _context.AllQueryable<Image> ()
                .OrderBy (m => m.Id)
                .Skip (startingFrom)
                .Take (count)
                .ToListAsync ();

            return result;
        }

        public async Task<IEnumerable<Image>> GetAllByFieldAsync (string fieldName, string fieldValue, CancellationToken cancellationToken = default) {
            var filter = Builders<Image>.Filter.Eq (fieldName, fieldValue);
            var result = await _context.GetCollection<Image> ().Find (filter).ToListAsync ();

            return result;
        }

        public async Task<Image> GetAsync (string id, CancellationToken cancellationToken = default) {
            return await _context.GetAsync<Image> (id);
        }
    }
}