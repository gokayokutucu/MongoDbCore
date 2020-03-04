using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Planet.MongoDbConsoleAppSample.Context;
using Planet.MongoDbConsoleAppSample.Enums;
using Planet.MongoDbConsoleAppSample.Models;

namespace Planet.MongoDbConsoleAppSample {
    public static class IndexConfigurations {

        public static async Task CreateAllIndexes (BlincatMongoDbContext context, CancellationToken cancellationToken = default) {
            await BookmarkIndexAsync (context.Bookmarks, cancellationToken);
        }
        public static async Task BookmarkIndexAsync (this IMongoCollection<Bookmark> collection, CancellationToken cancellationToken = default) {
            await CreateIndexAsync<Bookmark> (collection, (b => b.CreatedDate), null, IndexKeyType.Ascending, cancellationToken).ConfigureAwait (false);
        }

        //https://stackoverflow.com/questions/35019313/checking-if-an-index-exists-in-mongodb
        async static Task CreateIndexAsync<TEntity> (IMongoCollection<TEntity> collection, Expression<Func<TEntity, object>> field,
            CreateOneIndexOptions createOneIndexOptions = null, IndexKeyType indexKeyType = IndexKeyType.Ascending, CancellationToken cancellationToken = default) where TEntity : Entity {
            if (field == null)
                throw new ArgumentNullException ("Field expression cannot be empty");

            CreateIndexModel<TEntity> indexModel;
            switch (indexKeyType) {
                case IndexKeyType.Descending:
                    indexModel = new CreateIndexModel<TEntity> (Builders<TEntity>.IndexKeys.Descending (field));
                    break;
                case IndexKeyType.Text:
                    indexModel = new CreateIndexModel<TEntity> (Builders<TEntity>.IndexKeys.Text (field));
                    break;
                case IndexKeyType.Hashed:
                    indexModel = new CreateIndexModel<TEntity> (Builders<TEntity>.IndexKeys.Hashed (field));
                    break;
                default:
                    indexModel = new CreateIndexModel<TEntity> (Builders<TEntity>.IndexKeys.Ascending (field));
                    break;
            }

            await collection.Indexes.CreateOneAsync (indexModel, createOneIndexOptions, cancellationToken).ConfigureAwait (false);
        }

        async static Task CreateCombinedIndex<TEntity> (IMongoCollection<TEntity> collection, CreateOneIndexOptions createOneIndexOptions = null, IndexKeyType indexKeyType = IndexKeyType.Ascending, CancellationToken cancellationToken = default, params IndexKeysDefinition<TEntity>[] keys) where TEntity : Entity {
            if (keys == null)
                throw new ArgumentNullException ("Field expression cannot be empty");

            var indexModel = new CreateIndexModel<TEntity> (Builders<TEntity>.IndexKeys.Combine (keys));

            await collection.Indexes.CreateOneAsync (indexModel, createOneIndexOptions, cancellationToken).ConfigureAwait (false);
        }
    }
}