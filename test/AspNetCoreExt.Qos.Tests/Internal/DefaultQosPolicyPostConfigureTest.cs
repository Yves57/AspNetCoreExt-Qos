using AspNetCoreExt.Qos.Internal;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace AspNetCoreExt.Qos.Tests.Internal
{
    public class DefaultQosPolicyPostConfigureTest
    {
        [Fact]
        void PostConfigure()
        {
            var defaultResponse = new Mock<IQosRejectResponse>(MockBehavior.Strict);
            var initialResponse = new Mock<IQosRejectResponse>(MockBehavior.Strict);

            var policies = new List<QosPolicy>()
            {
                new QosPolicy("a") { RejectResponse = null },
                new QosPolicy("b") { RejectResponse = initialResponse.Object },
                new QosPolicy("c") { RejectResponse = null },
                new QosPolicy("d") { RejectResponse = initialResponse.Object },
            };

            var postConfigure = new DefaultQosPolicyPostConfigure(defaultResponse.Object);

            postConfigure.PostConfigure(policies);

            Assert.Collection(
                policies,
                p => Assert.Equal(defaultResponse.Object, p.RejectResponse),
                p => Assert.Equal(initialResponse.Object, p.RejectResponse),
                p => Assert.Equal(defaultResponse.Object, p.RejectResponse),
                p => Assert.Equal(initialResponse.Object, p.RejectResponse));
        }
    }
}
