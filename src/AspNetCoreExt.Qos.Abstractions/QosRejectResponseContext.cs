using Microsoft.AspNetCore.Http;

namespace AspNetCoreExt.Qos
{
    public class QosRejectResponseContext
    {
        public HttpContext HttpContext { get; set; }

        public QosPolicy Policy { get; set; }
    }
}
