using DMG.Examples.Testing.Api;
using DMG.Examples.Testing.IntegrationTests.Common;
using System.Net;
using Xunit.Abstractions;
using Flurl.Http;
using FluentAssertions;
using DMG.Examples.Testing.Domain.Models;

namespace DMG.Examples.Testing.IntegrationTests.Tests.UserControllerTests
{
    public class GetAllTests : UserControllerTestBase
    {
        public GetAllTests(
            ITestOutputHelper output,
            DataFixture dataFixture,
            CustomWebApplicationFactory<Program> apiFactory
        ) : base(output, dataFixture, apiFactory)
        {
        }

        [Fact(DisplayName = "When GetAll is called then response status code is ok and all users are returned")]
        public async Task When_GetAll_Is_Called_Then_Response_Status_Code_Is_Ok_And_All_Users_Are_Returned()
        {
            // Act
            var response = await BASE_ROUTE
                .GetAsync();

            // Assert
            response.StatusCode
                .Should()
                .Be((int)HttpStatusCode.OK);

            (await response
                .ResponseMessage
                .Content
                .ReadAsAsync<IEnumerable<User>>())
                .Should()
                .BeEquivalentTo(
                    _dataFixture.Users, 
                    options => options.Excluding(u => u.Id).WithoutStrictOrdering());
        }
    }
}
