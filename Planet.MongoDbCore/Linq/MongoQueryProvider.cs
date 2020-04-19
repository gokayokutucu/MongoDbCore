using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Misc;
using MongoDB.Driver.Linq;

namespace Planet.MongoDbCore.Linq
{
    public class MongoQueryProvider<T> : IMongoQueryProvider
    {
        private readonly IQueryContext<T> _queryContext;

        public MongoQueryProvider(IQueryContext<T> queryContext)
        {
            _queryContext = queryContext;
            CollectionNamespace = null;
            CollectionDocumentSerializer = null;
        }
        public IQueryable CreateQuery(Expression expression)
        {
            Ensure.IsNotNull(expression, nameof(expression));

            var elementType = expression.Type.GetElementType();

            try
            {
                return (IQueryable)Activator.CreateInstance(
                    typeof(Queryable<>).MakeGenericType(elementType), this, expression);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException ?? tie;
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new Queryable<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            try
            {
                return _queryContext.Execute(expression, false);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException ?? tie;
            }
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var result = _queryContext.Execute(expression, false);
            return (TResult)result;
        }

        public CollectionNamespace CollectionNamespace { get; }
        public IBsonSerializer CollectionDocumentSerializer { get; }

        public QueryableExecutionModel GetExecutionModel(Expression expression)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            var lambda = Expression.Lambda(expression);
            return (Task<TResult>)lambda.Compile().DynamicInvoke(null);
        }
    }
}
