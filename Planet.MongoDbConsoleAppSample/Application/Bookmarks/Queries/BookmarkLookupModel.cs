using System;
using System.Collections.Generic;
using AutoMapper;
using Planet.MongoDbConsoleAppSample.Enums;
using Planet.MongoDbConsoleAppSample.Mapping;
using Planet.MongoDbConsoleAppSample.Models;

namespace Planet.MongoDbConsoleAppSample.Application.Bookmarks.Queries {
    public class BookmarkLookupModel : IHaveCustomMapping {
        public string Title { get; private set; }
        public string Url { get; private set; }
        public BookmarkContentType ContentType { get; private set; }
        public List<ImageVoDTO> Images { get; set; }

        public void CreateMappings (Profile configuration) {
            configuration.CreateMap<Bookmark, BookmarkLookupModel> ();
        }

        public class ImageVoDTO {
            public string Id { get; private set; }
            public string Name { get; private set; }
            public string Url { get; private set; }
        }
    }
}