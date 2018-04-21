using AspNetCoreExt.Qos;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace QosSandbox
{
    public class MyCustomQosRejectResponse : QosRejectResponse
    {
        private readonly ILogger _logger;

        public MyCustomQosRejectResponse(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<MyCustomQosRejectResponse>();
        }

        public override Task WriteAsync(QosRejectResponseContext context)
        {
            _logger.LogInformation($"Policy {context.Policy.Name} rejected request '{context.HttpContext.Request.Path}'.");

            return base.WriteAsync(context);
        }
    }
}
