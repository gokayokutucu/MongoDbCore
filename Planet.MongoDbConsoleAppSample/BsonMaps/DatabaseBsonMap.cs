using MongoDB.Bson.Serialization.Conventions;
using Planet.MongoDbConsoleAppSample.Conventions;
using Planet.MongoDbConsoleAppSample.Models;

namespace Planet.MongoDbConsoleAppSample.BsonMaps {
    public static class DatabaseBsonMap {
        public static void Map () {
            var conventions = new ConventionPack ();
            conventions.Add (new LowerCaseConvention ());
            conventions.Add (new ImmutablePocoConvention ());
            // conventions.Add (new ImmutableAggregationConvention ());

            ConventionRegistry.Register (
                "mongoDbConventions",
                conventions,
                _ => true);

            EntityBaseBsonMap.Map ();
            DocumentBsonMap<Bookmark>.Map ();
        }
    }
}