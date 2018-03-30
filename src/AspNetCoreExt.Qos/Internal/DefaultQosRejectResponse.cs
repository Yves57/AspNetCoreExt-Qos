using System.Threading.Tasks;

namespace AspNetCoreExt.Qos.Internal
{
    public class DefaultQosRejectResponse : IQosRejectResponse
    {

        public Task WriteAsync(QosRejectResponseContext context)
        {
            context.HttpContext.Response.StatusCode = QosConstants.TooManyRequestHttpStatus;

            // TODO --> Add specific headers?

            return Task.CompletedTask;
        }
    }
}
