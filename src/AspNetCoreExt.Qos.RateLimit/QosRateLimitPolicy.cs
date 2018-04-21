using System;
using System.Collections.Generic;

namespace AspNetCoreExt.Qos.RateLimit
{
    public class QosRateLimitPolicy
    {
        public IEnumerable<string> UrlTemplates { get; set; }

        public object Key { get; set; }

        public TimeSpan Period { get; set; }

        public int MaxCount { get; set; }

        public bool Distributed { get; set; }
    }
}
