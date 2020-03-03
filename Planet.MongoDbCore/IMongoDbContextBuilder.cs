namespace Planet.MongoDbCore {
    public interface IMongoDbContextBuilder {
        string DatabaseName { get; set; }
        string Host { get; set; }
        int Port { get; set; }
        string Url { get; set; }
    }
}