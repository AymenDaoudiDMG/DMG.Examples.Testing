using DMG.Examples.Testing.Domain.Models;

namespace DMG.Examples.Testing.Services
{
    public interface IService
    {
    }

    public interface IService<TModel> : IService where TModel : ModelBase
    {
        Task<IEnumerable<TModel>> GetAllAsync();

        Task<TModel?> GetAsync(string id);

        Task<TModel> CreateAsync(TModel user);
    }
}