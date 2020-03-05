using System;
using System.Collections.Generic;
using MediatR;
using Planet.MongoDbConsoleAppSample.Enums;
using Planet.MongoDbConsoleAppSample.Models;

namespace Planet.MongoDbConsoleAppSample.Application.Bookmarks.Commands {
    public class UpsertBookmarkCommand : IRequest<string> {
        public string Id { get; set; }
        public string Url { get; private set; }
        public string Title { get; set; }
        public BookmarkContentType ContentType { get; private set; }
        public DetailType DetailType { get; private set; }
        public List<Image> Images { get; set; }
    }
}