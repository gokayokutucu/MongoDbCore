using MongoDB.Driver;

namespace Planet.MongoDbCore {
    public class MongoDbContextOptions {
        public MongoUrl Url { get; set; }
        public MongoClientSettings Settings { get; set; }
        public string DatabaseName { get; set; }
    }
}