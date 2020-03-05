using System.Collections.Generic;
using Planet.MongoDbConsoleAppSample.Application.Bookmarks.Queries;

namespace Planet.MongoDbConsoleAppSample.Application.Bookmarks.ViewModels {
    public class BookmarkListViewModel {
        public IList<BookmarkLookupModel> Bookmarks { get; set; }
    }
}