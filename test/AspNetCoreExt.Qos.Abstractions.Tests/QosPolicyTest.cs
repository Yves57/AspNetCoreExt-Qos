using System;
using Xunit;

namespace AspNetCoreExt.Qos.Abstractions.Tests
{
    public class QosPolicyTest
    {
        [Fact]
        public void NameCannotBeEmpty()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new QosPolicy("");
            });
        }
    }
}
