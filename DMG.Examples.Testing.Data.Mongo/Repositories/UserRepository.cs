using DMG.Examples.Testing.Domain.Models;
using UserEntity = DMG.Examples.Testing.Data.Mongo.Entities.User;
using DMG.Examples.Testing.Domain.Repositories;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DMG.Examples.Testing.Data.Mongo.Repositories
{
    public class UserRepository : RepositoryBase<UserEntity>, IUserRepository
    {
        public UserRepository(DbContext dbContext) : base(dbContext, "users")
        {      
        }

        public async Task<User> CreateAsync(User user)
        {
            var userDocument = new UserEntity
            {
                Name = user.Name,
                Age = user.Age
            };
            
            await _collection.InsertOneAsync(userDocument);

            var createdUser = new User
            {
                Id = userDocument.Id.ToString(),
                Name = userDocument.Name,
                Age = userDocument.Age
            };

            return createdUser;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var usersCursor = await _collection.FindAsync(new BsonDocument());
            var userDocuments = await usersCursor.ToListAsync();
            return userDocuments.Select(userDocument => new User
            {
                Id = userDocument.Id.ToString(),
                Name = userDocument.Name,
                Age = userDocument.Age
            });
        }

        public async Task<User?> GetAsync(string id)
        {
            var userDocument = await _collection.Find(new BsonDocument("_id", new ObjectId(id))).SingleOrDefaultAsync();
            var user = userDocument is null ? null : new User
            {
                Id = userDocument.Id.ToString(),
                Name = userDocument.Name,
                Age = userDocument.Age
            };
            return user;
        }
    }
}
