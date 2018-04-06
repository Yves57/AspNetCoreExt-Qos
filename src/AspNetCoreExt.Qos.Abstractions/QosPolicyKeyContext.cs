using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace AspNetCoreExt.Qos
{
    public class QosPolicyKeyContext
    {
        public HttpContext HttpContext { get; set; }

        public QosPolicy Policy { get; set; }

        public RouteTemplate RouteTemplate { get; set; }

        public RouteValueDictionary RouteValues { get; set; }
    }
}
