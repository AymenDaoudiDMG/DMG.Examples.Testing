using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace DMG.Examples.Testing.Data.Mongo.Entities
{
    public record User : DocumentBase
    {
        [BsonElement("name")]
        public required string Name { get; init; }

        [BsonElement("age")]
        public required int Age { get; init; }
    }
}