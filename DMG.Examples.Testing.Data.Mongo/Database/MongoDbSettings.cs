using Microsoft.Extensions.Configuration;

namespace DMG.Examples.Testing.Data.Mongo.Database
{
    public record MongoDbSettings
    {
        public string Url { get; private set; }
        public string DbUserName { get; private set; }
        public string DbPassword { get; private set; }
        public string DbName { get; private set; }

        public MongoDbSettings(string url, string dbUserName, string dbPassword, string dbName)
        {
            Url = url;
            DbUserName = dbUserName;
            DbPassword = dbPassword;
            DbName = dbName;
        }
    }
}
