using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Planet.MongoDbCore.Linq {
    /// <summary>
    /// An implementation of <see cref="IQueryProvider" /> for MongoDB.
    /// </summary>
    public interface IMongoQueryProvider : IQueryProvider {
        /// <summary>
        /// Executes the strongly-typed query represented by a specified expression tree.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="expression">An expression tree that represents a LINQ query.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The value that results from executing the specified query.</returns>
        Task<TResult> ExecuteAsync<TResult> (Expression expression, CancellationToken cancellationToken = default (CancellationToken));
    }
}