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
        private static readonly QosUrlTemplate MvcUrlTemplate = new QosUrlTemplate("POST", "foo/bar");

        private readonly List<QosPolicy> _policies;

        public MvcQosPolicyPostConfigureTest()
        {
            _policies = new List<QosPolicy>()
            {
                new QosPolicy("NoUrlTemplate"),
                new QosPolicy("OneUrlTemplate")
                {
                    UrlTemplates = new[] { new QosUrlTemplate("GET", "one/url") }
                }
            };
        }

        [Fact]
        public void AttributeOnMethod_NoUrlTemplate()
        {
            var apiProvider = CreateApiDescriptionProvider(typeof(TestAttributeOnMethod).GetMethod(nameof(TestAttributeOnMethod.NoUrl)));

            var postConfigure = new MvcQosPolicyPostConfigure(apiProvider);
            postConfigure.PostConfigure(_policies);

            Assert.Single(_policies[0].UrlTemplates);
            Assert.Contains(MvcUrlTemplate, _policies[0].UrlTemplates);
        }

        [Fact]
        public void AttributeOnMethod_OneUrlTemplate()
        {
            var apiProvider = CreateApiDescriptionProvider(typeof(TestAttributeOnMethod).GetMethod(nameof(TestAttributeOnMethod.OneUrl)));

            var postConfigure = new MvcQosPolicyPostConfigure(apiProvider);
            postConfigure.PostConfigure(_policies);

            Assert.Equal(2, _policies[1].UrlTemplates.Count());
            Assert.Contains(MvcUrlTemplate, _policies[1].UrlTemplates);
        }

        [Fact]
        public void AttributesOnMethodAndClass()
        {
            var apiProvider = CreateApiDescriptionProvider(typeof(TestAttributesOnMethodAndClass).GetMethod(nameof(TestAttributesOnMethodAndClass.NoUrl)));

            var postConfigure = new MvcQosPolicyPostConfigure(apiProvider);
            postConfigure.PostConfigure(_policies);

            Assert.Single(_policies[0].UrlTemplates);
            Assert.Contains(MvcUrlTemplate, _policies[0].UrlTemplates);
            Assert.Equal(2, _policies[1].UrlTemplates.Count());
            Assert.Contains(MvcUrlTemplate, _policies[1].UrlTemplates);
        }

        [Fact]
        public void AttributeOnBaseClass()
        {
            var apiProvider = CreateApiDescriptionProvider(typeof(TestAttributeOnBaseClassSub).GetMethod(nameof(TestAttributeOnBaseClassSub.NoUrl)));

            var postConfigure = new MvcQosPolicyPostConfigure(apiProvider);
            postConfigure.PostConfigure(_policies);

            Assert.Single(_policies[0].UrlTemplates);
            Assert.Contains(MvcUrlTemplate, _policies[0].UrlTemplates);
            Assert.Equal(2, _policies[1].UrlTemplates.Count());
            Assert.Contains(MvcUrlTemplate, _policies[1].UrlTemplates);
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
                        HttpMethod = MvcUrlTemplate.HttpMethod,
                        ActionDescriptor = new ControllerActionDescriptor()
                        {
                            MethodInfo = methodInfo,
                            AttributeRouteInfo = new AttributeRouteInfo()
                            {
                                Template = MvcUrlTemplate.Url
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
