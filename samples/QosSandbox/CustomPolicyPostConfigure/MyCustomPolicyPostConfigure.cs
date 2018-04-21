using AspNetCoreExt.Qos;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace QosSandbox.CustomPolicyPostConfigure
{
    public class MyCustomPolicyPostConfigure : IQosPolicyPostConfigure
    {
        public int Order => 1000;

        private readonly ILoggerFactory _loggerFactory;

        public MyCustomPolicyPostConfigure(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public void PostConfigure(IList<QosPolicy> policies)
        {
            foreach (var policy in policies)
            {
                if (policy.Name.StartsWith("Q"))
                {
                    policy.RejectResponse = new MyCustomQuotaRejectResponse(_loggerFactory);
                }
            }
        }
    }
}
