using DMG.Examples.Testing.UnitTests.Setups;

namespace DMG.Examples.Testing.UnitTests.Common
{
    [CollectionDefinition(nameof(AssemblyFixture))]
    public class AssemblyFixture :
        ICollectionFixture<MockData>,
        ICollectionFixture<UserRepositorySetup>
    {
    }
}