using Planet.MongoDbCore.Utility;

namespace Planet.MongoDbCore.Extensions {
    public static class MongoDbContextExtensions {
        public static string GetCollectionName (this System.Type s) {
            return new PluralizationServiceInstance ()
                .Pluralize (s.Name)
                .ToLower ();
        }
    }
}