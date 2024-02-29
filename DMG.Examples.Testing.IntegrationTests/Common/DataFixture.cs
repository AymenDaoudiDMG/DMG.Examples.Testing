using AutoFixture;
using DMG.Examples.Testing.Domain.Models;
using MongoDB.Driver;

namespace DMG.Examples.Testing.IntegrationTests.Common
{
    public class DataFixture
    {
        public List<User> Users { get; private set; }
        private readonly IFixture _fixture = new Fixture();

        public DataFixture()
        {
            _fixture.Customize<User>(composer => composer
                .FromFactory((string name, uint age) => new User
                {
                    Name = name,
                    Age = (int)(age == 0 ? 1 : age)
                })
                .Without(u => u.Id)
                .OmitAutoProperties());

            Users = _fixture.CreateMany<User>(20).ToList();
        }
    }
}
