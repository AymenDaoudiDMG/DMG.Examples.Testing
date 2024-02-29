using AutoFixture;
using DMG.Examples.Testing.Domain.Repositories;
using DMG.Examples.Testing.Services;
using DMG.Examples.Testing.UnitTests.Common;
using DMG.Examples.Testing.UnitTests.Setups;
using Moq;
using Xunit.Abstractions;

namespace DMG.Examples.Testing.UnitTests.Tests.UserServiceTests
{
    public class UserServiceTestBase : TestBase<IUserService>
    {
        protected readonly UserRepositorySetup _userRepositorySetup;

        public UserServiceTestBase(MockData mockData, ITestOutputHelper output) 
        : base(
            mockData, 
            output,
            sutFactory: f => f.Create<UserService>(),
            mockFactories: f => f.Freeze<Mock<IUserRepository>>()
        )
        {
            _userRepositorySetup = _fixture.Create<UserRepositorySetup>();
        }
    }
}