using DMG.Examples.Testing.Data.Mongo;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using DMG.Examples.Testing.Data.Mongo.Database;

namespace DMG.Examples.Testing.IntegrationTests.Common
{
    public class CustomWebApplicationFactory<TProgram>
        : WebApplicationFactory<TProgram> where TProgram : class
    {
        public IConfigurationRoot Configuration { get; set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .AddEnvironmentVariables()
                .Build();

            var mongodbSettings = new MongoDbSettings(
                Configuration["Database:MongoDb:Url"],
                Configuration["Database:MongoDb:DataBaseUser"],
                Configuration["Database:MongoDb:DataBasePassword"],
                Configuration["Database:MongoDb:DatabaseName"]
            );

            builder.ConfigureServices(services =>
            {
                services.Replace(ServiceDescriptor.Scoped<DbContext>(_ => new DbContext(mongodbSettings)));
            });

            builder.UseEnvironment("Test");
        }
    }
}