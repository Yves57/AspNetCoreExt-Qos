using AspNetCoreExt.Qos;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace QosSandbox.CustomPolicyPostConfigure
{
    public class MyCustomQuotaRejectResponse : QosRejectResponse
    {
        private readonly ILogger _logger;

        public MyCustomQuotaRejectResponse(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MyCustomQuotaRejectResponse>();
        }

        public override Task WriteAsync(QosRejectResponseContext context)
        {
            _logger.LogInformation($"Quota exceeded for '{context.HttpContext.Request.Path}'.");

            return base.WriteAsync(context);
        }
    }
}
