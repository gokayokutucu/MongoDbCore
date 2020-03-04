using MongoDB.Driver;

namespace Planet.MongoDbCore {
    public class MongoClientOptions {
        public MongoClientSettings Settings { get; set; }
        public MongoUrl Url { get; set; }
    }
}