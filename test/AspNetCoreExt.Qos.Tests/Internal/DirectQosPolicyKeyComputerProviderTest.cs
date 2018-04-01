using AspNetCoreExt.Qos.Internal;
using Moq;
using Xunit;

namespace AspNetCoreExt.Qos.Tests.Internal
{
    public class DirectQosPolicyKeyComputerProviderTest
    {
        [Fact]
        public void TryCreate()
        {
            var computer = new Mock<IQosPolicyKeyComputer>(MockBehavior.Strict);
            var provider = new DirectQosPolicyKeyComputerProvider();

            Assert.Equal(computer.Object, provider.TryCreate(computer.Object));
            Assert.Null(provider.TryCreate("@(\"abc\")"));
            Assert.Null(provider.TryCreate(null));
        }
    }
}
