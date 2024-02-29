using DMG.Examples.Testing.Domain.Models;
using DMG.Examples.Testing.Domain.Repositories;
using Moq;

namespace DMG.Examples.Testing.UnitTests.Setups
{
    public class UserRepositorySetup
    {
        public Mock<IUserRepository> UserRepositoryMock;

        public void GetAllAsyncSetup(Func<IEnumerable<User>> returnAction)
        {
            UserRepositoryMock
                .Setup(b => b.GetAllAsync())
                .ReturnsAsync(returnAction);
        }

        public void GetAsyncSetup(Func<string, User?> returnAction)
        {
            UserRepositoryMock
                .Setup(b => b.GetAsync(It.IsAny<string>()))
                .ReturnsAsync(returnAction);
        }

        public void CreateAsyncSetup(Func<User, User> returnAction)
        {
            UserRepositoryMock
                .Setup(b => b.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(returnAction);
        }
    }
}