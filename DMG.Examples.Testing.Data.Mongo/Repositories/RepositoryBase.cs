using DMG.Examples.Testing.Data.Mongo.Entities;
using MongoDB.Driver;

namespace DMG.Examples.Testing.Data.Mongo.Repositories
{
    public abstract class RepositoryBase<TEntity> where TEntity : DocumentBase
    {
        protected readonly DbContext _dbContext;
        protected readonly string _collectionName;
        protected readonly IMongoCollection<TEntity> _collection;
        

        public RepositoryBase(DbContext dbContext, string collectionName)
        {
            _dbContext = dbContext;
            _collectionName = collectionName;
            _collection = _dbContext.Database.GetCollection<TEntity>(_collectionName);
        }
    }
}