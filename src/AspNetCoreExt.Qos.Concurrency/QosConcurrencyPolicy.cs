using System.Collections.Generic;

namespace AspNetCoreExt.Qos.Concurrency
{
    public class QosConcurrencyPolicy
    {
        public IEnumerable<string> UrlTemplates { get; set; }

        public object Key { get; set; }

        public int MaxCount { get; set; }

        public bool Distributed { get; set; }
    }
}
