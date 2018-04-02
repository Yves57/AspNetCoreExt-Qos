using AspNetCoreExt.Qos.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace QosSandbox.Controllers
{
    [Route("api")]
    public class ValuesController : ControllerBase
    {
        [HttpGet("ratelimit/{id}")]
        public string GetRateLimit(int id)
        {
            return "value";
        }

        [HttpGet("quota1")]
        public string GetQuota1()
        {
            return new string('1', 10000);
        }

        [HttpGet("quota2")]
        public string GetQuota2()
        {
            return new string('2', 20000);
        }

        [HttpGet("ratelimitmvc")]
        [QosPolicy("R_MVC")]
        public string GetRateLimitMvc()
        {
            return "value";
        }
    }
}
