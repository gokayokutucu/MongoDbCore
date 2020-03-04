using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using Planet.MongoDbConsoleAppSample.Models;

namespace Planet.MongoDbConsoleAppSample.BsonMaps {
    public static class BookmarkBsonMap {
        public static void Map () {
            BsonClassMap.RegisterClassMap<Entity> (cm => {
                cm.AutoMap ();
                cm.MapIdProperty (c => c.Id)
                    .SetIdGenerator (StringObjectIdGenerator.Instance)
                    .SetSerializer (new StringSerializer (BsonType.ObjectId));
                cm.SetIsRootClass (true);
            });
            BsonClassMap.RegisterClassMap<Bookmark> (cm => {
                cm.AutoMap ();
                //OR you can map properties manually
                //cm.MapProperty (c => c.Title).SetElementName ("title");
                //cm.GetMemberMap (c => c.Title).SetIsRequired (true);
                //cm.MapProperty (c => c.Url).SetElementName ("url");
                //cm.MapMember (c => c.ExtractingType).SetSerializer (new EnumSerializer<ExtractingType> (BsonType.String));
                //cm.MapMember (c => c.Owners).SetSerializer (new ArraySerializer<List<Owner>> ());
            });
        }
    }
}