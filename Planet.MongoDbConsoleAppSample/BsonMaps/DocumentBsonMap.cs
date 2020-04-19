using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Entities.Core;

namespace Planet.MongoDbConsoleAppSample.BsonMaps {
    public static class DocumentBsonMap<T> {
        public static void Map () {
            if (!BsonClassMap.IsClassMapRegistered (typeof (T))) {
                BsonClassMap.RegisterClassMap<T> (cm => { cm.AutoMap (); });
            }
        }
    }
}