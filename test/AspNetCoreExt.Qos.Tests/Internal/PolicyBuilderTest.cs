using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using System.Linq;
using AspNetCoreExt.Qos.Internal;

namespace AspNetCoreExt.Qos.Tests.Internal
{
    public class PolicyBuilderTest
    {
        [Fact]
        public void Success()
        {
            var provider1 = new Mock<IQosPolicyProvider>(MockBehavior.Strict);
            provider1
                .SetupGet(p => p.Order)
                .Returns(1);
            provider1
                .Setup(p => p.GetPolicies())
                .Returns(new[]
                {
                    new QosPolicy("1A"),
                    new QosPolicy("1B")
                });

            var provider2 = new Mock<IQosPolicyProvider>(MockBehavior.Strict);
            provider2
                .SetupGet(p => p.Order)
                .Returns(2);
            provider2
                .Setup(p => p.GetPolicies())
                .Returns(new[]
                {
                    new QosPolicy("2A"),
                    new QosPolicy("2B")
                });

            var postConfigure1 = new Mock<IQosPolicyPostConfigure>(MockBehavior.Strict);
            postConfigure1
                .SetupGet(p => p.Order)
                .Returns(1);
            postConfigure1
                .Setup(p => p.PostConfigure(
                    It.Is<IList<QosPolicy>>(policies => policies
                        .Select(x => x.Name)
                        .SequenceEqual(new[] { "1A", "1B", "2A", "2B" }))))
                .Callback<IList<QosPolicy>>(policies => policies.RemoveAt(2));

            var postConfigure2 = new Mock<IQosPolicyPostConfigure>(MockBehavior.Strict);
            postConfigure2
                .SetupGet(p => p.Order)
                .Returns(2);
            postConfigure2
                .Setup(p => p.PostConfigure(
                    It.Is<IList<QosPolicy>>(policies => policies
                        .Select(x => x.Name)
                        .SequenceEqual(new[] { "1A", "1B", "2B" }))))
                .Verifiable();

            var policyProviders = new[]
            {
                provider2.Object,
                provider1.Object
            }; // Set reverse order to check if sorting is done
            var postConfigurators = new[]
            {
                postConfigure2.Object,
                postConfigure1.Object
            }; // Set reverse order to check if sorting is done

            var builder = new PolicyBuilder(policyProviders, postConfigurators);

            var result = builder.Build();

            Assert.Collection(
                result,
                p => Assert.Equal("1A", p.Policy.Name),
                p => Assert.Equal("1B", p.Policy.Name),
                p => Assert.Equal("2B", p.Policy.Name));
        }

        [Fact]
        public void DuplicateNames()
        {
            var provider = new Mock<IQosPolicyProvider>(MockBehavior.Strict);
            provider
                .SetupGet(p => p.Order)
                .Returns(1);
            provider
                .Setup(p => p.GetPolicies())
                .Returns(new[]
                {
                    new QosPolicy("A"),
                    new QosPolicy("B"),
                    new QosPolicy("A")
                });

            var builder = new PolicyBuilder(
                new[] { provider.Object },
                Enumerable.Empty<IQosPolicyPostConfigure>());

            Assert.Throws<Exception>(() => builder.Build());
        }
    }
}
