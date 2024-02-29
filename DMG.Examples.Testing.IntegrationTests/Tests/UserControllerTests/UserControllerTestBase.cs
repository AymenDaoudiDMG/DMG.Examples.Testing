using DMG.Examples.Testing.Api;
using DMG.Examples.Testing.IntegrationTests.Common;
using Xunit.Abstractions;

namespace DMG.Examples.Testing.IntegrationTests.Tests.UserControllerTests
{
    public class UserControllerTestBase : TestBase
    {
        protected const string BASE_ROUTE = "User";

        public UserControllerTestBase(
            ITestOutputHelper output, 
            DataFixture dataFixture, 
            CustomWebApplicationFactory<Program> apiFactory
        ) : base(output, dataFixture, apiFactory)
        {
        }
    }
}
