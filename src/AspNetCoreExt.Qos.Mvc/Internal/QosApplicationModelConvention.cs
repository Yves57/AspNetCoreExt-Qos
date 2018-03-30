using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AspNetCoreExt.Qos.Mvc.Internal
{
    public class QosApplicationModelConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            application.ApiExplorer.IsVisible = true;
        }
    }
}
