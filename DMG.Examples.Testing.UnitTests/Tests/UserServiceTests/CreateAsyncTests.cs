using AutoFixture;
using DMG.Examples.Testing.Domain.Models;
using DMG.Examples.Testing.UnitTests.Common;
using FluentAssertions;
using Xunit.Abstractions;

namespace DMG.Examples.Testing.UnitTests.Tests.UserServiceTests
{
    public class CreateAsyncTests : UserServiceTestBase
    {
        public CreateAsyncTests(MockData mockData, ITestOutputHelper output) : base(mockData, output)
        {
        }

        [Fact(DisplayName = "When CreateAsync is called then no exception is thrown")]
        public async void When_CreateAsync_Is_Called_Then_No_Exception_Is_Thrown()
        {
            //Arrange
            var user = _fixture.Build<User>()
                .With(u => u.Id, (++_mockData.UserIdCounter).ToString())
                .Create();

            _userRepositorySetup.CreateAsyncSetup((u) =>
            {
                // Adding user to DB
                _mockData.Users.Add(u);

                // Returning the added user from DB
                return _mockData.Users.Single(usr => usr.Id == _mockData.UserIdCounter.ToString());
            });

            //Act
            Func<User, Task<User>> func = _sut.CreateAsync;

            //Assert             
            await func.Invoking(f => f(user)).Should().NotThrowAsync<Exception>();
        }

        [Fact(DisplayName = "When CreateAsync is called with input user then user added to the source list")]
        public async void When_CreateAsync_Is_Called_With_Input()
        {
            //Arrange
            var user = _fixture.Build<User>()
                .Without(u => u.Id)
                .Create();

            _userRepositorySetup.CreateAsyncSetup((u) =>
            {
                var userToInsert = user with { Id = (++_mockData.UserIdCounter).ToString() };
                _mockData.Users.Add(userToInsert);
                return _mockData.Users.Single(usr => usr.Id == _mockData.UserIdCounter.ToString());
            });

            //Act
            var createdUser = await _sut.CreateAsync(user);

            //Assert             
            createdUser
                .Should()
                .NotBeNull()
                .And
                .Match<User>(u => u.Id == _mockData.UserIdCounter.ToString() && u.Name == user.Name && u.Age == user.Age);
        }
    }
}
