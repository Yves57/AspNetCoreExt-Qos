using AspNetCoreExt.Qos;
using AspNetCoreExt.Qos.Concurrency;
using AspNetCoreExt.Qos.Quota;
using AspNetCoreExt.Qos.RateLimit;
using AspNetCoreExt.Qos.Vip;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using QosSandbox.CustomPolicy;
using QosSandbox.CustomPolicyPostConfigure;
using System;

namespace QosSandbox
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IQosRejectResponse, MyCustomQosRejectResponse>();
            services.AddSingleton<IQosPolicyProvider, MyCustomPolicyProvider>();
            services.AddSingleton<IQosPolicyPostConfigure, MyCustomPolicyPostConfigure>();

            services.AddQosRedisStore(o => o.Configuration = "localhost");

            services.AddQos();
            services.AddExpressionPolicyKeyComputer();

            services.Configure<QosVipOptions>(Configuration.GetSection("Vip"));
            services.AddQosVip();

            services.Configure<QosConcurrencyOptions>(Configuration.GetSection("Concurrency"));
            services.AddQosConcurrency();

            services.Configure<QosRateLimitOptions>(Configuration.GetSection("RateLimit"));
            services.PostConfigure<QosRateLimitOptions>(o =>
            {
                o.Policies.Add("R_HARDCODED", new QosRateLimitPolicy()
                {
                    MaxCount = 2,
                    Period = new TimeSpan(0, 0, 30),
                    UrlTemplates = new[] { "/api/ratelimit2" },
                    Key = QosPolicyKeyComputer.Create(c => c.HttpContext.Connection.RemoteIpAddress.ToString())
                });
            });

            services.AddQosRateLimit();

            services.Configure<QosQuotaOptions>(Configuration.GetSection("Quota"));

            services.AddQosQuota();

            services.AddQosMvc();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseQosVip();
            app.UseQos();

            app.UseMvc();
        }
    }
}
