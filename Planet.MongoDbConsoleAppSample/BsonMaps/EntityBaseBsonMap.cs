using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using Planet.MongoDbConsoleAppSample.Models;

namespace Planet.MongoDbConsoleAppSample.BsonMaps {
    public static class EntityBaseBsonMap {
        public static void Map () {
            //Mapping for class which are derived from Entity object
            //ATTENTION: Don't forget to map non-entity based class
            if (!BsonClassMap.IsClassMapRegistered (typeof (Entity))) {
                BsonClassMap.RegisterClassMap<Entity> (cm => {
                    cm.AutoMap ();
                    cm.MapIdProperty (c => c.Id)
                        .SetIdGenerator (StringObjectIdGenerator.Instance)
                        .SetSerializer (new StringSerializer (BsonType.ObjectId));
                    cm.SetIsRootClass (true);
                });
            }
        }
    }
}