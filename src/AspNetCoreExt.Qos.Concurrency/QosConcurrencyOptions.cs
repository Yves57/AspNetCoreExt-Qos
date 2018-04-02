using System.Collections.Generic;

namespace AspNetCoreExt.Qos.Concurrency
{
    public class QosConcurrencyOptions
    {
        public IDictionary<string, QosConcurrencyPolicy> Policies { get; set; }
    }
}
