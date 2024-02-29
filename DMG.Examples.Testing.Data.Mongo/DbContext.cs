using DMG.Examples.Testing.Data.Mongo.Database;
using MongoDB.Driver;

namespace DMG.Examples.Testing.Data.Mongo
{
    public class DbContext
    {
        public IMongoDatabase Database { get; init; }

        public DbContext(MongoDbSettings mongoDbSettings)
        {
            var connectionString = new MongoUrlBuilder
            {
                Server = new MongoServerAddress(mongoDbSettings.Url),
                Username = mongoDbSettings.DbUserName,
                Password = mongoDbSettings.DbPassword,
                DatabaseName = mongoDbSettings.DbName
            }.ToMongoUrl();

            var client = new MongoClient(connectionString);
            Database = client.GetDatabase(mongoDbSettings.DbName);
        }
    }
}