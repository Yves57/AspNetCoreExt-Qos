using AspNetCoreExt.Qos.ExpressionPolicyKeyComputer.Internal;
using Xunit;

namespace AspNetCoreExt.Qos.ExpressionPolicyKeyComputer.Tests
{
    public class ExpressionPolicyKeyComputerProviderTest
    {
        [Theory]
        [InlineData("@(\"abc\")")]
        [InlineData("@(   \"abc\"    )")]
        public void TryCreate_Expression_Success(string source)
        {
            var provider = new ExpressionPolicyKeyComputerProvider();

            var computer = provider.TryCreate(source);

            Assert.NotNull(computer);
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
            var provider = new ExpressionPolicyKeyComputerProvider();

            var computer = provider.TryCreate(source);

            Assert.Null(computer);
        }

        [Theory]
        [InlineData("@{ return \"abc\"; }")]
        [InlineData("@{{ return \"abc\"; }}")]
        public void TryCreate_Method_Success(string source)
        {
            var provider = new ExpressionPolicyKeyComputerProvider();

            var computer = provider.TryCreate(source);

            Assert.NotNull(computer);
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
            var provider = new ExpressionPolicyKeyComputerProvider();

            var computer = provider.TryCreate(source);

            Assert.Null(computer);
        }
    }
}
