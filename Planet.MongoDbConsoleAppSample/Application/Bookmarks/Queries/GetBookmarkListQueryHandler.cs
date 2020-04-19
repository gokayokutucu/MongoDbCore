using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Planet.MongoDbConsoleAppSample.Application.Bookmarks.ViewModels;
using Planet.MongoDbConsoleAppSample.Repositories;

namespace Planet.MongoDbConsoleAppSample.Application.Bookmarks.Queries {
    public class GetBookmarkListQueryHandler : IRequestHandler<GetBookmarkListQuery, BookmarkListViewModel> {
        private readonly IBookmarkRepository _bookmarkRepository;
        private readonly IMapper _mapper;

        public GetBookmarkListQueryHandler (IBookmarkRepository bookmarkRepository, IMapper mapper) {
            _bookmarkRepository = bookmarkRepository;
            _mapper = mapper;
        }

        public async Task<BookmarkListViewModel> Handle (GetBookmarkListQuery request, CancellationToken cancellationToken) {
            var bookmarkQueryable = _bookmarkRepository.AllQueryable ().Where (a => !a.IsDeleted);
            var bookmarkList = await bookmarkQueryable.ToListAsync (cancellationToken);

            return new BookmarkListViewModel {
                Bookmarks = _mapper.Map<IList<BookmarkLookupModel>> (bookmarkList)
            };
        }
    }
}