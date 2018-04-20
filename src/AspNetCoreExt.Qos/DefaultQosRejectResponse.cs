using System.Threading.Tasks;

namespace AspNetCoreExt.Qos
{
    public class QosRejectResponse : IQosRejectResponse
    {
        public virtual Task WriteAsync(QosRejectResponseContext context)
        {
            context.HttpContext.Response.StatusCode = QosConstants.TooManyRequestHttpStatus;

            return Task.CompletedTask;
        }
    }
}
