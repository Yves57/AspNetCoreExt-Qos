using AspNetCoreExt.Qos.Internal;
using Moq;
using Xunit;

namespace AspNetCoreExt.Qos.Tests.Internal
{
    public class DirectQosPolicyKeyEvaluatorProviderTest
    {
        [Fact]
        public void TryCreate()
        {
            var evaluator = new Mock<IQosPolicyKeyEvaluator>(MockBehavior.Strict);
            var provider = new DirectQosPolicyKeyEvaluatorProvider();

            Assert.Equal(evaluator.Object, provider.TryCreate(evaluator.Object));
            Assert.Null(provider.TryCreate("@(\"abc\")"));
            Assert.Null(provider.TryCreate(null));
        }
    }
}
