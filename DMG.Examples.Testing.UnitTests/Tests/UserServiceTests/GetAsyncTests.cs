using DMG.Examples.Testing.Domain.Models;
using DMG.Examples.Testing.UnitTests.Common;
using FluentAssertions;
using Xunit.Abstractions;

namespace DMG.Examples.Testing.UnitTests.Tests.UserServiceTests
{
    public class GetAsyncTests : UserServiceTestBase
    {
        public GetAsyncTests(MockData mockData, ITestOutputHelper output) : base(mockData, output)
        {
        }

        [Theory(DisplayName = "When GetAsync is called then no excepton is thrown")]
        [InlineData(null)]
        [InlineData("0")]
        [InlineData("1")]
        [InlineData("5")]
        [InlineData("10")]
        [InlineData("15")]
        [InlineData("20")]
        [InlineData("2147483647")]
        public async void When_Get_Async_Is_Called_Then_No_Exception_Is_Thrown(string id)
        {
            //Arrange
            _userRepositorySetup.GetAsyncSetup((id) => _mockData.Users.SingleOrDefault(u => u.Id == id));

            //Act
            Func<string, Task<User?>> func = _sut.GetAsync;

            //Assert             
            await func
                .Invoking(f => f(id))
                .Should()
                .NotThrowAsync<Exception>();
        }

        [Theory(DisplayName = "When GetAsync is called with existing Id then correct user is returned")]
        [InlineData("1")]
        [InlineData("5")]
        [InlineData("10")]
        [InlineData("15")]
        [InlineData("20")]
        public async void When_Get_Async_Is_Called_With_Existing_Id_Then_Correct_User_Is_Returned(string id)
        {
            //Arrange
            _userRepositorySetup.GetAsyncSetup((id) => _mockData.Users.SingleOrDefault(u => u.Id == id));
            var expectedUser = _mockData.Users.SingleOrDefault(u => u.Id == id);

            //Act
            var user = await _sut.GetAsync(id);

            //Assert         
            user.Should()
                .NotBeNull().And
                .BeEquivalentTo(expectedUser);
        }

        [Theory(DisplayName = "When GetAsync is called with unexisting Id then null is returned")]
        [InlineData(null)]
        [InlineData("0")]
        [InlineData("2147483647")]
        public async void When_Get_Async_Is_Called_With_Unexisting_Id_Then_Null_Is_Returned(string id)
        {
            //Arrange
            _userRepositorySetup.GetAsyncSetup((id) => _mockData.Users.SingleOrDefault(u => u.Id == id));

            //Act
            var user = await _sut.GetAsync(id);

            //Assert             
            user.Should()
                .BeNull();
        }
    }
}
