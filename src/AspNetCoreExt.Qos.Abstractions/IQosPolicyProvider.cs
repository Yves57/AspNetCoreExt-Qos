using System.Collections.Generic;

namespace AspNetCoreExt.Qos
{
    public interface IQosPolicyProvider
    {
        int Order { get; }

        IEnumerable<QosPolicy> GetPolicies();
    }
}
