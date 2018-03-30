using Xunit;

namespace AspNetCoreExt.Qos.ExpressionPolicyKeyComputer.Tests
{
    public class ExpressionPolicyKeyComputerTest
    {
        [Theory]
        [InlineData("\"abc\"", "abc")]
        [InlineData("\"abc\".Substring(1)", "bc")] // System assembly
        [InlineData("new string(\"abc\".First(), 2)", "aa")] // Linq and IEnumerable<> assemblies
        public void Expression(string expression, string expectedKey)
        {
            var computer = new Internal.ExpressionPolicyKeyComputer(expression);

            Assert.Equal(expectedKey, computer.GetKey(new QosPolicyKeyContext()));
        }

        [Fact]
        public void Method()
        {
            var computer = new Internal.ExpressionPolicyKeyComputer("{ if (context != null) return \"ok\"; else return \"problem\"; }");

            Assert.Equal("ok", computer.GetKey(new QosPolicyKeyContext()));
        }
    }
}
