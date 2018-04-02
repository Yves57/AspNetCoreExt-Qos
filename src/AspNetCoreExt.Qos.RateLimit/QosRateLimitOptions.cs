using System.Collections.Generic;

namespace AspNetCoreExt.Qos.RateLimit
{
    public class QosRateLimitOptions
    {
        public IDictionary<string, QosRateLimitPolicy> Policies { get; set; }
    }
}
