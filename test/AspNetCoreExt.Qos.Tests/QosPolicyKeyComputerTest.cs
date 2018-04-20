using Xunit;

namespace AspNetCoreExt.Qos.Tests
{
    public class QosPolicyKeyComputerTest
    {
        [Fact]
        public void Create()
        {
            var keyComputer = QosPolicyKeyComputer.Create(c => c.Policy.Name);

            var result = keyComputer.GetKey(new QosPolicyKeyContext()
            {
                Policy = new QosPolicy("ABC")
            });

            Assert.Equal("ABC", result);
        }
    }
}
