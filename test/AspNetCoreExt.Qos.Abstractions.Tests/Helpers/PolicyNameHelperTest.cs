using AspNetCoreExt.Qos.Abstractions.Helpers;
using Xunit;

namespace AspNetCoreExt.Qos.Abstractions.Tests.Helpers
{
    public class PolicyNameHelperTest
    {
        [Theory]
        [InlineData("Abbb")]
        [InlineData("B")]
        [InlineData("A___Ddd123dZ")]
        public void ValidName(string name)
        {
            Assert.True(PolicyNameHelper.IsValid(name));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("1_DigitFirst")]
        [InlineData("InvalidChar-aaa")]
        [InlineData("InvalidChar*aaa")]
        public void InvalidName(string name)
        {
            Assert.False(PolicyNameHelper.IsValid(name));
        }
    }
}
