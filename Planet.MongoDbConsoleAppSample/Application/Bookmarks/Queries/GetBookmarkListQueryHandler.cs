using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
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
            var bookmarks = await _bookmarkRepository.GetAllAsync (cancellationToken);

            return new BookmarkListViewModel {
                Bookmarks = _mapper.Map<IList<BookmarkLookupModel>> (bookmarks)
            };
        }
    }
}