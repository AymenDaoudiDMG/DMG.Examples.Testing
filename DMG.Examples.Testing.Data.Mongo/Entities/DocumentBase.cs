using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DMG.Examples.Testing.Data.Mongo.Entities
{
    public abstract record DocumentBase
    {
        [BsonId]
        public ObjectId Id { get; init; }
    }
}
