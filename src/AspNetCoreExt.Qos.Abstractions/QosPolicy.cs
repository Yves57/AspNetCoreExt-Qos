﻿using System;
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

        public IEnumerable<QosUrlTemplate> UrlTemplates { get; set; }

        public IQosPolicyKeyEvaluator Key { get; set; }

        public IQosRejectResponse RejectResponse { get; set; }

        public IQosPolicyGate Gate { get; set; }
    }
}
