using Microsoft.AspNetCore.Http;

namespace AspNetCoreExt.Qos
{
    public class QosGateEnterContext
    {
        public HttpContext HttpContext { get; set; }

        public string Key { get; set; }
    }
}
