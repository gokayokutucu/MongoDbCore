using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Planet.MongoDbCore.Linq {
    public class AsyncCursor<TEntity> : IAsyncCursor<TEntity> {
        public void Dispose () {
            GC.SuppressFinalize (this);
        }

        public bool MoveNext (CancellationToken cancellationToken = new CancellationToken ()) {
            return false;
        }

        public Task<bool> MoveNextAsync (CancellationToken cancellationToken = new CancellationToken ()) {
            return Task.FromResult (false);
        }

        public IEnumerable<TEntity> Current => new List<TEntity> ().AsReadOnly ();
    }
}