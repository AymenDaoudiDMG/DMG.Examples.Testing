
using DMG.Examples.Testing.Data.Mongo;
using DMG.Examples.Testing.Data.Mongo.Database;
using DMG.Examples.Testing.Data.Mongo.Repositories;
using DMG.Examples.Testing.Domain.Repositories;
using DMG.Examples.Testing.Services;
using dotenv.net;

namespace DMG.Examples.Testing.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var mongodbSettings = new MongoDbSettings(
                builder.Configuration["Database:MongoDb:Url"],
                builder.Configuration["Database:MongoDb:DataBaseUser"],
                builder.Configuration["Database:MongoDb:DataBasePassword"],
                builder.Configuration["Database:MongoDb:DatabaseName"]
            );

            builder.Services
                .AddScoped<DbContext>(_ => new DbContext(mongodbSettings))
                .AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IUserService, UserService>()
                .AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
