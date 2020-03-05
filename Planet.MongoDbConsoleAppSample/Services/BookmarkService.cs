using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Planet.MongoDbConsoleAppSample.Repositories;

namespace Planet.MongoDbConsoleAppSample.Services {
    public class BookmarkService : IHostedService {
        private readonly IBookmarkRepository _repo;

        public BookmarkService (IBookmarkRepository repo) {
            _repo = repo;
        }

        public Task StartAsync (CancellationToken cancellationToken) {
            throw new System.NotImplementedException ();
        }

        public Task StopAsync (CancellationToken cancellationToken) {
            throw new System.NotImplementedException ();
        }
    }
}