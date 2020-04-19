using System.Collections.Generic;
using System.Linq.Expressions;

namespace Planet.MongoDbCore.Linq
{
    public interface IQueryContext<out T>
    {
        IEnumerable<T> Execute(Expression expression, bool isEnumerable);
    }
}
