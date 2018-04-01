using AspNetCoreExt.Qos.Internal;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreExt.Qos.Tests.Internal
{
    public class DefaultQosRejectResponseTest
    {
        [Fact]
        public async Task Write()
        {
            var httpContext = new DefaultHttpContext();
            var response = new DefaultQosRejectResponse();

            await response.WriteAsync(new QosRejectResponseContext()
            {
                HttpContext = httpContext
            });

            Assert.Equal(QosConstants.TooManyRequestHttpStatus, httpContext.Response.StatusCode);
        }
    }
}
