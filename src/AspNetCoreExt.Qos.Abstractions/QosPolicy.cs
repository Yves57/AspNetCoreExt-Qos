using System;
using System.Collections.Generic;

namespace AspNetCoreExt.Qos
{
    public sealed class QosPolicy
    {
        public QosPolicy(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Cannot be empty.", nameof(name));
            }

            Name = name;
        }

        public string Name { get; }

        public int Order { get; set; }

        public IEnumerable<string> UrlTemplates { get; set; }

        public IQosPolicyKeyComputer Key { get; set; }

        public IQosRejectResponse RejectResponse { get; set; }

        public IQosPolicyGate Gate { get; set; }
    }
}
