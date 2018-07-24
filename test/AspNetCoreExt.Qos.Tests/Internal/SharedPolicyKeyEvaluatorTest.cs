using AspNetCoreExt.Qos.Internal;
using Xunit;

namespace AspNetCoreExt.Qos.Tests.Internal
{
    public class SharedPolicyKeyEvaluatorTest
    {
        [Fact]
        void GetKey()
        {
            var evaluator = new SharedPolicyKeyEvaluator();
            var key = evaluator.GetKey(new QosPolicyKeyContext()
            {
                Policy = new QosPolicy("MyPolicy")
            });

            Assert.Equal("QoS_Shared_MyPolicy", key);
        }
    }
}
