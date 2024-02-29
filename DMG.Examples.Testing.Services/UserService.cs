using DMG.Examples.Testing.Domain.Models;
using DMG.Examples.Testing.Domain.Repositories;

namespace DMG.Examples.Testing.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        public Task<User> CreateAsync(User user) => _userRepository.CreateAsync(user);

        public Task<IEnumerable<User>> GetAllAsync() => _userRepository.GetAllAsync();

        public Task<User?> GetAsync(string id) => _userRepository.GetAsync(id);
    }
}