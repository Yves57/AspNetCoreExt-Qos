using Microsoft.AspNetCore.Http;

namespace AspNetCoreExt.Qos
{
    public class QosGateExitContext
    {
        public HttpContext HttpContext { get; set; }

        public string Key { get; set; }

        public object GateData { get; set; }

        public bool Reject { get; set; }
    }
}
