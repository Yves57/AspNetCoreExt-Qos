using Xunit;

namespace AspNetCoreExt.Qos.ExpressionPolicyKeyEvaluator.Tests
{
    public class ExpressionPolicyKeyEvaluatorTest
    {
        [Theory]
        [InlineData("\"abc\"", "abc")]
        [InlineData("\"abc\".Substring(1)", "bc")] // System assembly
        [InlineData("new string(\"abc\".First(), 2)", "aa")] // Linq and IEnumerable<> assemblies
        public void Expression(string expression, string expectedKey)
        {
            var evaluator = new Internal.ExpressionPolicyKeyEvaluator(expression);

            Assert.Equal(expectedKey, evaluator.GetKey(new QosPolicyKeyContext()));
        }

        [Fact]
        public void Method()
        {
            var evaluator = new Internal.ExpressionPolicyKeyEvaluator("{ if (context != null) return \"ok\"; else return \"problem\"; }");

            Assert.Equal("ok", evaluator.GetKey(new QosPolicyKeyContext()));
        }
    }
}
