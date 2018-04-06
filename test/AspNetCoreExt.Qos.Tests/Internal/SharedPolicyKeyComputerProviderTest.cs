using AspNetCoreExt.Qos.Internal;
using Xunit;

namespace AspNetCoreExt.Qos.Tests.Internal
{
    public class SharedPolicyKeyComputerProviderTest
    {
        [Fact]
        public void CreateCorrectComputer()
        {
            var provider = new SharedPolicyKeyComputerProvider();

            Assert.IsType<SharedPolicyKeyComputer>(provider.TryCreate("*"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("**")]
        [InlineData("@(\"abc\")")]
        [InlineData(12)]
        public void IgnoreUnknownSyntaxes(object rawKeyComputer)
        {
            var provider = new SharedPolicyKeyComputerProvider();

            Assert.Null(provider.TryCreate(rawKeyComputer));
        }
    }
}
