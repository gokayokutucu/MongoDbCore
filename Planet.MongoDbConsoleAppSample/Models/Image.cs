using System;

namespace Planet.MongoDbConsoleAppSample.Models {
    public class Image : Entity {
        protected Image () {
            IsDeleted = false;
        }

        public Image (string fileName, string url, string bookmarkId) {
            FileName = fileName;
            Url = url;
            BookmarkId = bookmarkId;
            CreatedDate = DateTime.UtcNow;
            IsDeleted = false;
        }

        public string FileName { get; private set; }
        public string Url { get; private set; }
        public string BookmarkId { get; private set; }
    }
}