using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Planet.MongoDbCore.Linq {
    public class QueryContext<T> : IQueryContext<T> {
        public IEnumerable<T> Execute (Expression expression, bool isEnumerable) {
            return Enumerable.Empty<T> ();
        }
    }
}