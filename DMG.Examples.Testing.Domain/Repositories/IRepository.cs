using DMG.Examples.Testing.Domain.Models;

namespace DMG.Examples.Testing.Domain.Repositories
{
    public interface IRepository
    {
    }

    public interface IRepository<TModel> : IRepository where TModel : ModelBase
    {
        Task<IEnumerable<TModel>> GetAllAsync();

        Task<TModel?> GetAsync(string id);

        Task<TModel> CreateAsync(TModel user);
    }
}