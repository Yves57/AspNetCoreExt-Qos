using System.Collections.Generic;

namespace AspNetCoreExt.Qos.Quota
{
    public class QosQuotaOptions
    {
        public IEnumerable<QosQuotaPolicy> Policies { get; set; }
    }
}
