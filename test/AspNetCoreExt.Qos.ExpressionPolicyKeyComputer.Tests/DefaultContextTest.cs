using AspNetCoreExt.Qos.ExpressionPolicyKeyComputer.Internal.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Net;
using Xunit;

namespace AspNetCoreExt.Qos.ExpressionPolicyKeyComputer.Tests
{
    public class DefaultContextTest
    {
        [Fact]
        public void Properties()
        {
            var ip = "101.102.103.104";
            var httpContext = new DefaultHttpContext();

            httpContext.Request.Method = "PATCH";
            httpContext.Request.Host = new HostString("www.example.fr", 1234);
            httpContext.Request.Path = "/foo/bar";
            httpContext.Request.QueryString = new QueryString("?p1=111&p2=222");
            httpContext.Request.Scheme = "http";
            httpContext.Connection.RemoteIpAddress = IPAddress.Parse(ip);

            httpContext.Request.Headers.Add("h1", "val1");
            httpContext.Request.Headers.Add("h2", "val2");
            httpContext.Request.Headers.Add("h3", new StringValues(new string[] { "val3a", "val3b" }));

            var contextDate = new DateTime(2018, 3, 30);
            var context = new DefaultContext(httpContext, contextDate);
            var request = context.Request;
            var headers = context.Request.Headers;

            Assert.Equal(contextDate, context.Timestamp);

            //Assert.Equal("", request.UrlTemplate);
            Assert.Equal("PATCH", request.Method);
            Assert.Null(request.Certificate);
            Assert.Equal(ip, request.IpAddress);
            Assert.Equal("http://www.example.fr:1234/foo/bar?p1=111&p2=222", request.Url);

            Assert.Equal(new[] { "Host", "h1", "h2", "h3" }, headers.Keys);
            Assert.Collection(
                headers.Values,
                v => Assert.Single(v, "www.example.fr:1234"),
                v => Assert.Single(v, "val1"),
                v => Assert.Single(v, "val2"),
                v => Assert.Equal(new[] { "val3a", "val3b" }, v));
            Assert.Equal(new[] { "val3a", "val3b" }, headers["h3"]);
            Assert.Null(headers["blabla"]);
            Assert.True(headers.ContainsKey("h2"));
            Assert.False(headers.ContainsKey("blabla"));
            Assert.True(headers.TryGetValue("h1", out var values));
            Assert.Single(values, "val1");
            Assert.False(headers.TryGetValue("blabla", out values));
            Assert.Null(values);
            Assert.Equal("val3a,val3b", headers.GetValueOrDefault("h3", "abc"));
            Assert.Equal("abc", headers.GetValueOrDefault("blabla", "abc"));
        }
    }
}
