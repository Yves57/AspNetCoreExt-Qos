using AspNetCoreExt.Qos.Mvc.Internal;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Xunit;

namespace AspNetCoreExt.Qos.Mvc.Tests.Internal
{
    public class QosApplicationModelConventionTest
    {
        [Fact]
        public void SetApiExplorerActive()
        {
            var application = new ApplicationModel();
            var convention = new QosApplicationModelConvention();

            Assert.Null(application.ApiExplorer.IsVisible);

            convention.Apply(application);

            Assert.True(application.ApiExplorer.IsVisible);
        }
    }
}
