using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Planet.MongoDbCore.Linq
{
    public class Queryable<T> : IOrderedMongoQueryable<T>
    {
        private readonly IMongoQueryProvider _queryProvider;
        public Queryable(IQueryContext<T> queryContext)
        {
            _queryProvider = new MongoQueryProvider<T>(queryContext);
            Initialize(_queryProvider, null);
        }

        public Queryable(IMongoQueryProvider provider)
        {
            _queryProvider = provider;
            Initialize(provider, null);
        }

        internal Queryable(IMongoQueryProvider provider, Expression expression)
        {
            _queryProvider = provider;
            Expression = expression;
            Initialize(provider, expression);
        }

        private void Initialize(IMongoQueryProvider provider, Expression expression)
        {
            if (expression != null && !typeof(IQueryable<T>).
                IsAssignableFrom(expression.Type))
                throw new ArgumentException(
                    $"Not assignable from {expression.Type}", "expression");

            Provider = provider ?? throw new ArgumentNullException("provider");
            Expression = expression ?? Expression.Constant(this);
        }

        public Type ElementType => typeof(T);
        public Expression Expression { get; internal set; }
        public IQueryProvider Provider { get; internal set; }

        public QueryableExecutionModel GetExecutionModel()
        {
            throw new NotImplementedException();
        }

        public IAsyncCursor<T> ToCursor(CancellationToken cancellationToken = new CancellationToken())
        {
            return new AsyncCursor<T>();
        }

        public Task<IAsyncCursor<T>> ToCursorAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var result = Task.Run(() => new AsyncCursor<T>() as IAsyncCursor<T>, cancellationToken);
            return result;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return (Provider.Execute<IEnumerable<T>>(Expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (Provider.Execute<IEnumerable>(Expression)).GetEnumerator();
        }
    }
}
