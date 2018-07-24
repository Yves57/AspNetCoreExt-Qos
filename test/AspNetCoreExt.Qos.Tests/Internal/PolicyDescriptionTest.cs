using AspNetCoreExt.Qos.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using Xunit;

namespace AspNetCoreExt.Qos.Tests.Internal
{
    public class PolicyDescriptionTest
    {
        public static IEnumerable<object[]> GetTryUrlMatching_Success_Values()
        {
            yield return new object[]
            {
                new QosUrlTemplate("POST", "/foo/bar"),
                "Post", // Lower case
                "/foo/BAR" // Upper case
            };
            yield return new object[]
            {
                new QosUrlTemplate("POST", "/foo/{param1}/bar/{param2}"),
                "POST",
                "/foo/12/bar/abc"
            };
            yield return new object[]
            {
                new QosUrlTemplate(null, "/foo/bar"), // Accept all methods
                "PATCH",
                "/foo/bar"
            };
        }

        [Theory]
        [MemberData(nameof(GetTryUrlMatching_Success_Values))]
        public void TryUrlMatching_Success(QosUrlTemplate urlTemplate, string httpMethod, string url)
        {
            var policy = new QosPolicy("Foo")
            {
                UrlTemplates = new[] { urlTemplate }
            };
            var description = new PolicyDescription(policy);

            var success = description.TryUrlMatching(httpMethod, new PathString(url), new RouteValueDictionary(), out var routeTemplate);

            Assert.True(success);
            Assert.NotNull(routeTemplate);
        }

        public static IEnumerable<object[]> GetTryUrlMatching_Fail_Values()
        {
            yield return new object[]
            {
                new QosUrlTemplate("POST", "/foo/bar"),
                "GET", // Bad method
                "/foo/bar"
            };
            yield return new object[]
            {
                new QosUrlTemplate("POST", "/foo/{param1}/bar/{param2}"),
                "POST",
                "/foo/12/bar" // Missing parameter
            };
        }

        [Theory]
        [MemberData(nameof(GetTryUrlMatching_Fail_Values))]
        public void TryUrlMatching_Fail(QosUrlTemplate urlTemplate, string httpMethod, string url)
        {
            var policy = new QosPolicy("Foo")
            {
                UrlTemplates = new[] { urlTemplate }
            };
            var description = new PolicyDescription(policy);

            var success = description.TryUrlMatching(httpMethod, new PathString(url), new RouteValueDictionary(), out var routeTemplate);

            Assert.False(success);
            Assert.Null(routeTemplate);
        }
    }
}
