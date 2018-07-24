using AspNetCoreExt.Qos.ExpressionPolicyKeyEvaluator.Internal;
using Xunit;

namespace AspNetCoreExt.Qos.ExpressionPolicyKeyEvaluator.Tests
{
    public class ExpressionPolicyKeyEvaluatorProviderTest
    {
        [Theory]
        [InlineData("@(\"abc\")")]
        [InlineData("@(   \"abc\"    )")]
        public void TryCreate_Expression_Success(string source)
        {
            var provider = new ExpressionPolicyKeyEvaluatorProvider();

            var evaluator = provider.TryCreate(source);

            Assert.NotNull(evaluator);
        }

        [Theory]
        [InlineData("  @(\"abc\")")]
        [InlineData("@ (\"abc\")")]
        [InlineData("@\"abc\")")]
        [InlineData("@(\"abc\"")]
        [InlineData("@(\"abc\"}")]
        [InlineData("@{\"abc\")")]
        public void TryCreate_Expression_Fail(string source)
        {
            var provider = new ExpressionPolicyKeyEvaluatorProvider();

            var evaluator = provider.TryCreate(source);

            Assert.Null(evaluator);
        }

        [Theory]
        [InlineData("@{ return \"abc\"; }")]
        [InlineData("@{{ return \"abc\"; }}")]
        public void TryCreate_Method_Success(string source)
        {
            var provider = new ExpressionPolicyKeyEvaluatorProvider();

            var evaluator = provider.TryCreate(source);

            Assert.NotNull(evaluator);
        }

        [Theory]
        [InlineData("   @{ return \"abc\"; }")]
        [InlineData("@ { return \"abc\"; }")]
        [InlineData("@ return \"abc\"; }")]
        [InlineData("@{ return \"abc\";")]
        [InlineData("@( return \"abc\"; }")]
        [InlineData("@{ return \"abc\"; )")]
        public void TryCreate_Method_Fail(string source)
        {
            var provider = new ExpressionPolicyKeyEvaluatorProvider();

            var evaluator = provider.TryCreate(source);

            Assert.Null(evaluator);
        }
    }
}
