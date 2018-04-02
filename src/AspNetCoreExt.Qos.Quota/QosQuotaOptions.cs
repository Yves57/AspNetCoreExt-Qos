using System.Collections.Generic;

namespace AspNetCoreExt.Qos.Quota
{
    public class QosQuotaOptions
    {
        public IDictionary<string, QosQuotaPolicy> Policies { get; set; }
    }
}
