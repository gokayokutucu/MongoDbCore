using System;
using System.Linq.Expressions;
using Planet.MongoDbConsoleAppSample.Enums;
using Planet.MongoDbConsoleAppSample.Models;

namespace Planet.MongoDbConsoleAppSample.Application.Bookmarks.Queries {
    public class BookmarkDetailModel {
        public string Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public BookmarkContentType ContentType { get; set; }
        public DetailType DetailType { get; set; }

        public static Expression<Func<Bookmark, BookmarkDetailModel>> Projection {
            get {
                return bookmark => new BookmarkDetailModel {
                    Id = bookmark.Id,
                };
            }
        }

        public static BookmarkDetailModel Create (Bookmark bookmark) {
            return Projection.Compile ().Invoke (bookmark);
        }
    }
}