using DMG.Examples.Testing.Domain.Models;
using DMG.Examples.Testing.UnitTests.Common;
using ExpectedObjects;
using FluentAssertions;
using Xunit.Abstractions;

namespace DMG.Examples.Testing.UnitTests.Tests.UserServiceTests
{
    public class GetAllAsyncTests : UserServiceTestBase
    {
        public GetAllAsyncTests(MockData mockData, ITestOutputHelper output) : base(mockData, output)
        {
        }

        [Fact(DisplayName = "When GetAllAsync is called then no exception is thrown")]
        public async void When_GetAllAsync_Is_Called_Then_No_Exception_Is_Thrown()
        {
            //Arrange
            _userRepositorySetup.GetAllAsyncSetup(() => _mockData.Users);

            //Act
            Func<Task<IEnumerable<User>>> func = _sut.GetAllAsync;

            //Assert             
            await func.Should().NotThrowAsync<Exception>();
        }

        [Fact(DisplayName = "When GetAllAsync is called then all users are returned")]
        public async void When_GetAllAsync_Is_Called_Then_All_Users_Are_Returned()
        {
            //Arrange
            _userRepositorySetup.GetAllAsyncSetup(() => _mockData.Users);
            var expectedUsers = _mockData.Users.ToExpectedObject();

            //Act
            var result = await _sut.GetAllAsync();

            //Assert             
            expectedUsers.ShouldEqual(result);
        }
    }
}