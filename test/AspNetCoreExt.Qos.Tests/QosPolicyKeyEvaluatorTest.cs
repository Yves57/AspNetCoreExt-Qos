using Xunit;

namespace AspNetCoreExt.Qos.Tests
{
    public class QosPolicyKeyEvaluatorTest
    {
        [Fact]
        public void Create()
        {
            var keyEvaluator = QosPolicyKeyEvaluator.Create(c => c.Policy.Name);

            var result = keyEvaluator.GetKey(new QosPolicyKeyContext()
            {
                Policy = new QosPolicy("ABC")
            });

            Assert.Equal("ABC", result);
        }
    }
}
