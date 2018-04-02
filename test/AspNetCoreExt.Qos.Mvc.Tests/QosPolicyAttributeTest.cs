using System;
using Xunit;

namespace AspNetCoreExt.Qos.Mvc.Tests
{
    public class QosPolicyAttributeTest
    {
        [Fact]
        public void ValidName()
        {
            var policyAttribute = new QosPolicyAttribute("Policy_1");
            Assert.Equal("Policy_1", policyAttribute.PolicyName);
        }

        [Fact]
        public void InvalidName()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new QosPolicyAttribute("111");
            });
        }
    }
}
