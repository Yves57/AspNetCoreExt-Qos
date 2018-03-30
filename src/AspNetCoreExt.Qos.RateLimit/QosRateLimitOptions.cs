using System.Collections.Generic;

namespace AspNetCoreExt.Qos.RateLimit
{
    public class QosRateLimitOptions
    {
        public IEnumerable<QosRateLimitPolicy> Policies { get; set; }
    }
}
