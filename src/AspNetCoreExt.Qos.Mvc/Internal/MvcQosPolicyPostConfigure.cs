using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;

namespace AspNetCoreExt.Qos.Mvc.Internal
{
    public class MvcQosPolicyPostConfigure : IQosPolicyPostConfigure
    {
        public MvcQosPolicyPostConfigure(IApiDescriptionGroupCollectionProvider apiProvider)
        {
        }

        public int Order => -1000;

        public void PostConfigure(IList<QosPolicy> policies)
        {
            throw new NotImplementedException();
        }
    }
}
