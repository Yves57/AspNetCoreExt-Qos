using System;
using System.Net.Http;
using Xunit;

namespace AspNetCoreExt.Qos.Abstractions.Tests
{
    public class QosUrlTemplateTest
    {
        [Fact]
        public void Constructor_EmptyUrl()
        {
            Assert.Throws<FormatException>(() =>
            {
                new QosUrlTemplate("GET", "");
            });
        }

        [Theory]
        [InlineData("*", null, "*")]
        [InlineData("/foo/bar", null, "/foo/bar")]
        [InlineData("Get /foo/bar", "GET", "/foo/bar")]
        [InlineData("post foo", "POST", "foo")]
        public void Parse_Success(string input, string expectedMethod, string expectedUrl)
        {
            var template = QosUrlTemplate.Parse(input);

            if (expectedMethod == null)
            {
                Assert.Null(template.HttpMethod);
            }
            else
            {
                Assert.Equal(expectedMethod, template.HttpMethod);
            }
            Assert.Equal(expectedUrl, template.Url);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" /foo/bar")]
        [InlineData("/foo/bar ")]
        [InlineData("post /foo/bar third")]
        public void Parse_Error(string input)
        {
            Assert.Throws<FormatException>(() =>
            {
                QosUrlTemplate.Parse(input);
            });
        }
    }
}
