using AspNetCoreExt.Qos.Internal;
using Xunit;

namespace AspNetCoreExt.Qos.Tests.Internal
{
    public class SharedPolicyKeyEvaluatorProviderTest
    {
        [Fact]
        public void CreateCorrectEvaluator()
        {
            var provider = new SharedPolicyKeyEvaluatorProvider();

            Assert.IsType<SharedPolicyKeyEvaluator>(provider.TryCreate("*"));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("**")]
        [InlineData("@(\"abc\")")]
        [InlineData(12)]
        public void IgnoreUnknownSyntaxes(object rawKeyEvaluator)
        {
            var provider = new SharedPolicyKeyEvaluatorProvider();

            Assert.Null(provider.TryCreate(rawKeyEvaluator));
        }
    }
}
