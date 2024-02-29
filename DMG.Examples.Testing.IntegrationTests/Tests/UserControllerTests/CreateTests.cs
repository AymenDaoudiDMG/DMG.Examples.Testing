using AutoFixture;
using DMG.Examples.Testing.Api;
using DMG.Examples.Testing.Domain.Models;
using DMG.Examples.Testing.IntegrationTests.Common;
using FluentAssertions;
using Flurl.Http;
using System.Net;
using Xunit.Abstractions;

namespace DMG.Examples.Testing.IntegrationTests.Tests.UserControllerTests
{
    public class CreateTests : UserControllerTestBase
    {
        public CreateTests(
            ITestOutputHelper output,
            DataFixture dataFixture,
            CustomWebApplicationFactory<Program> apiFactory
        ) : base(output, dataFixture, apiFactory)
        {
        }

        [Fact]
        public async Task When_Create_Is_Called_Then_Response_Status_Code_Is_CreateAt()
        {
            //Arrange
            var user = _fixture.Build<User>()
                .Without(u => u.Id)
                .Create();

            // Act
            var response = await BASE_ROUTE
                .PostJsonAsync(user);

            // Assert
            response.StatusCode
                .Should()
                .Be((int)HttpStatusCode.Created);

            (await response
                .ResponseMessage
                .Content
                .ReadAsAsync<User>())
                .Should()
                .BeEquivalentTo(
                    user,
                    options => options.Excluding(u => u.Id));
        }
    }
}
