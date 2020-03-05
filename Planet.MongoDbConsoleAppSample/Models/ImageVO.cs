using System.Collections.Generic;
using Planet.MongoDbConsoleAppSample.SeedWork;

namespace Planet.MongoDbConsoleAppSample.Models {
    public class ImageVO : ValueObject {
        private ImageVO () { }

        public ImageVO (string imageId, string name, string url) {
            ImageId = imageId;
            Name = name;
            Url = url;
        }
        public string ImageId { get; private set; }
        public string Name { get; private set; }
        public string Url { get; private set; }
        protected override IEnumerable<object> GetAtomicValues () {
            yield return ImageId;
            yield return Name;
            yield return Url;
        }
    }
}