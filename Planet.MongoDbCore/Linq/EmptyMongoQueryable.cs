using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Planet.MongoDbCore.Linq {
    public class EmptyMongoQueryable<T> : IMongoQueryable<T> {
        public EmptyMongoQueryable () {
            _expression = Expression.Constant (this, typeof (IMongoQueryable<T>));
        }

        private readonly Expression _expression;
        public Expression Expression => _expression;
        public Type ElementType => typeof (T);
        public IQueryProvider Provider => new MongoQueryProvider<T> (new QueryContext<T> ());
        public IEnumerator<T> GetEnumerator () {
            var results = (IEnumerable<T>) Provider.Execute (_expression);
            return results.GetEnumerator ();
        }

        public QueryableExecutionModel GetExecutionModel () =>
        throw new NotImplementedException ();
        public IAsyncCursor<T> ToCursor (CancellationToken cancellationToken = default) =>
        throw new NotImplementedException ();
        public Task<IAsyncCursor<T>> ToCursorAsync (CancellationToken cancellationToken = default) =>
        throw new NotImplementedException ();
        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator ();
    }
}