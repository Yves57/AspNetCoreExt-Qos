using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Xunit;

namespace AspNetCoreExt.Qos.Tests
{
    public class QosRejectResponseTest
    {
        [Fact]
        public async Task Write()
        {
            var httpContext = new DefaultHttpContext();
            var response = new QosRejectResponse();

            await response.WriteAsync(new QosRejectResponseContext()
            {
                HttpContext = httpContext
            });

            Assert.Equal(QosConstants.TooManyRequestHttpStatus, httpContext.Response.StatusCode);
        }
    }
}
