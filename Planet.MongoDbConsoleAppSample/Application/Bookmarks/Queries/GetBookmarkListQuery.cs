using MediatR;
using Planet.MongoDbConsoleAppSample.Application.Bookmarks.ViewModels;

namespace Planet.MongoDbConsoleAppSample.Application.Bookmarks.Queries {
    public class GetBookmarkListQuery : IRequest<BookmarkListViewModel> { }
}