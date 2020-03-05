using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Planet.MongoDbConsoleAppSample.SeedWork {
    public interface IRepository<TEntity> where TEntity : class {
        IUnitOfWork UnitOfWork { get; }
        Task<bool> DeleteAsync (string id, CancellationToken cancellationToken = default);
        Task<TEntity> GetAsync (string id, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllAsync (CancellationToken cancellationToken = default);
    }
}