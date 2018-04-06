using AspNetCoreExt.Qos.Internal;
using Xunit;

namespace AspNetCoreExt.Qos.Tests.Internal
{
    public class SharedPolicyKeyComputerTest
    {
        [Fact]
        void GetKey()
        {
            var computer = new SharedPolicyKeyComputer();
            var key = computer.GetKey(new QosPolicyKeyContext()
            {
                Policy = new QosPolicy("MyPolicy")
            });

            Assert.Equal("QoS_Shared_MyPolicy", key);
        }
    }
}
