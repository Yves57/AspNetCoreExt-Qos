using System;
using System.Collections.Generic;

namespace AspNetCoreExt.Qos.Quota
{
    public class QosQuotaPolicy
    {
        public string Name { get; set; }

        public IEnumerable<string> UrlTemplates { get; set; }

        public object Key { get; set; }

        public TimeSpan Period { get; set; }

        public long MaxCount { get; set; }

        public bool Distributed { get; set; }
    }
}
