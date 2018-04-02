using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Reflection;
using AspNetCoreExt.Qos.Mvc.Internal;
using Xunit;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Routing;

namespace AspNetCoreExt.Qos.Mvc.Tests.Internal
{
    public class MvcQosPolicyPostConfigureTest
    {
        private static readonly string MvcUrlTemplate = "foo/bar";

        private readonly List<QosPolicy> _policies;

        public MvcQosPolicyPostConfigureTest()
        {
            _policies = new List<QosPolicy>()
            {
                new QosPolicy("NoUrlTemplate"),
                new QosPolicy("OneUrlTemplate")
                {
                    UrlTemplates = new[] { "one/url" }
                }
            };
        }

        [Fact]
        public void AttributeOnMethod_NoUrlTemplate()
        {
            var apiProvider = CreateApiDescriptionProvider(typeof(TestAttributeOnMethod).GetMethod(nameof(TestAttributeOnMethod.NoUrl)));

            var postConfigure = new MvcQosPolicyPostConfigure(apiProvider);
            postConfigure.PostConfigure(_policies);

            Assert.Equal(1, _policies[0].UrlTemplates.Count());
            Assert.True(_policies[0].UrlTemplates.Contains(MvcUrlTemplate));
        }

        [Fact]
        public void AttributeOnMethod_OneUrlTemplate()
        {
            var apiProvider = CreateApiDescriptionProvider(typeof(TestAttributeOnMethod).GetMethod(nameof(TestAttributeOnMethod.OneUrl)));

            var postConfigure = new MvcQosPolicyPostConfigure(apiProvider);
            postConfigure.PostConfigure(_policies);

            Assert.Equal(2, _policies[1].UrlTemplates.Count());
            Assert.True(_policies[1].UrlTemplates.Contains(MvcUrlTemplate));
        }

        [Fact]
        public void AttributesOnMethodAndClass()
        {
            var apiProvider = CreateApiDescriptionProvider(typeof(TestAttributesOnMethodAndClass).GetMethod(nameof(TestAttributesOnMethodAndClass.NoUrl)));

            var postConfigure = new MvcQosPolicyPostConfigure(apiProvider);
            postConfigure.PostConfigure(_policies);

            Assert.Equal(1, _policies[0].UrlTemplates.Count());
            Assert.True(_policies[0].UrlTemplates.Contains(MvcUrlTemplate));
            Assert.Equal(2, _policies[1].UrlTemplates.Count());
            Assert.True(_policies[1].UrlTemplates.Contains(MvcUrlTemplate));
        }

        [Fact]
        public void AttributeOnBaseClass()
        {
            var apiProvider = CreateApiDescriptionProvider(typeof(TestAttributeOnBaseClassSub).GetMethod(nameof(TestAttributeOnBaseClassSub.NoUrl)));

            var postConfigure = new MvcQosPolicyPostConfigure(apiProvider);
            postConfigure.PostConfigure(_policies);

            Assert.Equal(1, _policies[0].UrlTemplates.Count());
            Assert.True(_policies[0].UrlTemplates.Contains(MvcUrlTemplate));
            Assert.Equal(2, _policies[1].UrlTemplates.Count());
            Assert.True(_policies[1].UrlTemplates.Contains(MvcUrlTemplate));
        }

        [Fact]
        public void AttributesWithSameName()
        {
            var apiProvider = CreateApiDescriptionProvider(typeof(TestAttributesWithSameName).GetMethod(nameof(TestAttributesWithSameName.NoUrl)));

            var postConfigure = new MvcQosPolicyPostConfigure(apiProvider);

            Assert.Throws<Exception>(() =>
            {
                postConfigure.PostConfigure(_policies);
            });
        }

        private IApiDescriptionGroupCollectionProvider CreateApiDescriptionProvider(MethodInfo methodInfo)
        {
            var descriptionGroup = new ApiDescriptionGroup(
                "MyGroup",
                new List<ApiDescription>()
                {
                    new ApiDescription()
                    {
                        ActionDescriptor = new ControllerActionDescriptor()
                        {
                            MethodInfo = methodInfo,
                            AttributeRouteInfo = new AttributeRouteInfo()
                            {
                                Template = MvcUrlTemplate
                            }
                        }
                    }
                });

            var groupProvider = new Mock<IApiDescriptionGroupCollectionProvider>(MockBehavior.Strict);
            groupProvider
                .SetupGet(p => p.ApiDescriptionGroups)
                .Returns(new ApiDescriptionGroupCollection(new[] { descriptionGroup }, 1));

            return groupProvider.Object;
        }
    }

    public class TestAttributeOnMethod
    {
        [QosPolicy("NoUrlTemplate")]
        public void NoUrl()
        {
        }

        [QosPolicy("OneUrlTemplate")]
        public void OneUrl()
        {
        }
    }

    [QosPolicy("OneUrlTemplate")]
    public class TestAttributesOnMethodAndClass
    {
        [QosPolicy("NoUrlTemplate")]
        public void NoUrl()
        {
        }
    }

    [QosPolicy("OneUrlTemplate")]
    public abstract class TestAttributeOnBaseClassBase
    {
    }

    [QosPolicy("NoUrlTemplate")]
    public class TestAttributeOnBaseClassSub : TestAttributeOnBaseClassBase
    {
        public void NoUrl()
        {
        }
    }

    [QosPolicy("NoUrlTemplate")]
    public class TestAttributesWithSameName
    {
        [QosPolicy("NoUrlTemplate")]
        public void NoUrl()
        {
        }
    }
}
