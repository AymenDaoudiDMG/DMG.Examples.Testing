using AutoFixture;
using DMG.Examples.Testing.Domain.Models;

namespace DMG.Examples.Testing.UnitTests.Common
{
    public class MockData
    {
        private readonly IFixture _fixture = new Fixture();
        public int UserIdCounter { get; set; } = 0;
        public List<User> Users { get; private set; }

        public MockData()
        {
            Init();
        }

        private void Init()
        {
            _fixture.Customize<User>(composer => composer
                .FromFactory((string name, uint age) => new User
                {
                    Id = (++UserIdCounter).ToString(),
                    Name = name,
                    Age = (int)(age == 0 ? 1 : age)
                })
                .OmitAutoProperties());

            Users = _fixture.CreateMany<User>(20).ToList();
        }
    }
}
