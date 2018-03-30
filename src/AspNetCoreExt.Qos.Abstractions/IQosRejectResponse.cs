using System.Threading.Tasks;

namespace AspNetCoreExt.Qos
{
    public interface IQosRejectResponse
    {
        Task WriteAsync(QosRejectResponseContext context);
    }
}
