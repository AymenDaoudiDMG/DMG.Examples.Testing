using AutoFixture;
using AutoFixture.AutoMoq;
using DMG.Examples.Testing.Services;
using Xunit.Abstractions;

namespace DMG.Examples.Testing.UnitTests.Common
{
    [Collection(nameof(AssemblyFixture))]
    public abstract class TestBase<TSut> where TSut : IService
    {
        protected readonly ITestOutputHelper _output;
        protected readonly MockData _mockData;
        protected readonly IFixture _fixture;
        protected readonly TSut _sut;

        public TestBase(
            MockData mockData, 
            ITestOutputHelper output,
            Func<IFixture, TSut> sutFactory,
            params Action<IFixture>[] mockFactories
        )
        {
            _output = output;
            _mockData = mockData;
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            foreach (var mockFactory in mockFactories)
                mockFactory.Invoke(_fixture);
            
            _sut = sutFactory.Invoke(_fixture);
        }
    }
}
