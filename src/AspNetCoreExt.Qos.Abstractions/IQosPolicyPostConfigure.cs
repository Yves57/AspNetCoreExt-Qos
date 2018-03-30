using System.Collections.Generic;

namespace AspNetCoreExt.Qos
{
    public interface IQosPolicyPostConfigure
    {
        int Order { get; }

        void PostConfigure(IList<QosPolicy> policies);
    }
}
