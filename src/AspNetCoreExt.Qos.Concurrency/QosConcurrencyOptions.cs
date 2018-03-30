using System.Collections.Generic;

namespace AspNetCoreExt.Qos.Concurrency
{
    public class QosConcurrencyOptions
    {
        public IEnumerable<QosConcurrencyPolicy> Policies { get; set; }
    }
}
