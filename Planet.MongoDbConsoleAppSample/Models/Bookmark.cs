using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Planet.MongoDbConsoleAppSample.Enums;

namespace Planet.MongoDbConsoleAppSample.Models {
    public class Bookmark : Entity {
        private readonly List<Image> _images;
        public IReadOnlyCollection<Image> Images => _images;
        protected Bookmark () {
            _images = new List<Image> ();

            IsDeleted = false;
        }
        public Bookmark (IReadOnlyCollection<Image> images) {
            _images = new List<Image> ();
            _images.AddRange (images);
        }
        public Bookmark (string url, string title, string createdBy, BookmarkContentType contentType = BookmarkContentType.Website,
            DetailType detailType = DetailType.Simple, Identifier identifier = Identifier.Auto, string id = default) {
            _images = _images ?? new List<Image> ();

            this.Id = (string.IsNullOrEmpty (id) && identifier == Identifier.New) ? ObjectId.GenerateNewId ().ToString () : id;

            Url = url;
            Title = title;
            ContentType = contentType;
            DetailType = detailType;

            IsDeleted = false;
            CreatedBy = createdBy;
            CreatedDate = DateTime.UtcNow;
        }

        public void AddImage (string fileName, string url, string bookmarkId, string createdBy) {
            var image = new Image (fileName, url, bookmarkId, createdBy);
            _images.Add (image);
        }

        public string Url { get; private set; }
        public string Title { get; set; }
        public BookmarkContentType ContentType { get; private set; }
        public DetailType DetailType { get; private set; }
    }
}