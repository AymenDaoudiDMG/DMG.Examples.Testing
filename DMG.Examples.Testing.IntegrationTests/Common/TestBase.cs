using AutoFixture;
using AutoFixture.AutoMoq;
using DMG.Examples.Testing.Api;
using Flurl.Http;
using Flurl;
using Xunit.Abstractions;
using Flurl.Http.Configuration;
using DMG.Examples.Testing.Data.Mongo.Database;
using DMG.Examples.Testing.Data.Mongo;
using MongoDB.Driver;
using UserEntity = DMG.Examples.Testing.Data.Mongo.Entities.User;

namespace DMG.Examples.Testing.IntegrationTests.Common
{
    [Collection(nameof(AssemblyFixture))]
    public class TestBase : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        protected readonly IFixture _fixture;
        protected readonly ITestOutputHelper _output;
        protected readonly DataFixture _dataFixture;
        protected readonly CustomWebApplicationFactory<Program> _sut; //can be named also _apiFactory

        public TestBase(
            ITestOutputHelper output,
            DataFixture dataFixture,
            CustomWebApplicationFactory<Program> sut
        )
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _output = output;
            _dataFixture = dataFixture;
            _sut = sut;
            
            FlurlHttp.Configure(settings =>
            {
                settings.FlurlClientFactory = new TestClientFactory(_sut.CreateClient());
            });
            SeedInitialDataToDatabase();
        }

        private class TestClientFactory(HttpClient _client) : IFlurlClientFactory
        {
            public void Dispose()
            {
                _client.Dispose();
            }

            public IFlurlClient Get(Url url)
            {
                var flurlClient = new FlurlClient(_client);
                return flurlClient;
            }
        }

        private void SeedInitialDataToDatabase()
        {
            var mongodbSettings = new MongoDbSettings(
                _sut.Configuration["Database:MongoDb:Url"],
                _sut.Configuration["Database:MongoDb:DataBaseUser"],
                _sut.Configuration["Database:MongoDb:DataBasePassword"],
                _sut.Configuration["Database:MongoDb:DatabaseName"]
            );

            var dbContext = new DbContext(mongodbSettings);
            var usersCollection = dbContext.Database.GetCollection<UserEntity>("users");
            usersCollection.DeleteMany(_ => true);

            usersCollection.InsertMany(_dataFixture.Users.Select(u => new UserEntity
            {
                Name = u.Name,
                Age = u.Age
            }));
        }
    }
}
