
# AspNetCoreExt-Qos

ASP.NET Core QoS is a set of high-performance and highly customizable middlewares that allow to set different limits on requests (quotas, rates).
All packages are available in Nuget.

## Simple example

**Startup.cs**
```csharp
public void ConfigureServices(IServiceCollection services)
{
     // Add base QoS middleware
    services.AddQos();

    // Allows to use C# expressions directly in the 'appsettings.json' file
    services.AddQosExpressionPolicyKeyEvaluator();

    // Add middleware that allows special request to bypass the QoS policies
    services.Configure<QosVipOptions>(Configuration.GetSection("Vip"));
    services.AddQosVip();

    // Add concurrency policies
    services.Configure<QosConcurrencyOptions>(Configuration.GetSection("Concurrency"));
    services.AddQosConcurrency();

    // Add rate-limit policies
    services.Configure<QosRateLimitOptions>(Configuration.GetSection("RateLimit"));
    services.PostConfigure<QosRateLimitOptions>(o =>
    {
        o.Policies.Add("R_HARDCODED", new QosRateLimitPolicy()
        {
            MaxCount = 2,
            Period = new TimeSpan(0, 0, 30),
            UrlTemplates = new[] { "/api/ratelimit2" },
            Key = QosPolicyKeyEvaluator.Create(c => c.HttpContext.Connection.RemoteIpAddress.ToString())
        });
    });
        
    services.AddQosRateLimit();

    // Add quotas
    services.Configure<QosQuotaOptions>(Configuration.GetSection("Quota"));
    services.AddQosQuota();
}

public void Configure(IApplicationBuilder app, IHostingEnvironment env)
{
    app.UseQosVip();
    app.UseQos();
}
```

**appsettings.json**
```json
  "Vip": {
    "IpAddresses": [
      "11.22.33.44",
      "22.33.44.55"
    ]
  },
  
  "Concurrency": {
    "Policies": {
      "C1": {
        "UrlTemplates": [ "*" ],
        "Key": "@(context.Request.IpAddress)",
        "MaxCount": 2
      }
    }
  },

  "RateLimit": {
    "Policies": {
      "R1": {
        "UrlTemplates": [ "POST /api/ratelimit/{id}" ],
        "Key": "@(context.Request.IpAddress)",
        "MaxCount": 3,
        "Period":  "0:0:10"
      }
    }
  },

  "Quota": {
    "Policies": {
      "Q1": {
        "UrlTemplates": [ "/api/quota1", "/api/quota2" ],
        "Key": "@(context.Request.Url)",
        "MaxCount": 300,
        "Period": "0:0:30",
        "Distributed": false
      }
    }

```

In the example above, there are several policies:
- Requests with two specific IP addresses bypass all QoS policies.
- It is forbidden to process more than 2 simultaneous requests from the same IP address.
- The route 'POST /api/ratelimit/{id}' accepts only 3 requests per 10 seconds from a same IP address.
- The routes '/api/quota1' and '/api/quota2' accept only 300KB per 30 seconds.
- Other routes has no specific policy (except the concurreny policy that is applied to all routes).

For more detailed information, see the example web site in the Github sources.

## Policy parameters

You can set different parameters to setup the policies:
- The URL templates indicates the routes to check. The special `"*"` URL is used as wildcard. If an HTTP method is set as first parameter, it is used to restrict the check. No method set means "every method is accepted".
- The counters are separated depending on the 'key'. Several formats are accepted to set the key (see below).
- The period defines a time window that start when the counter increment is done.
- Max count is the limit (does not exists in the concurrency policy). It is expressed in KB for the quotas.
- The Distributed property can be used in conjunction with Redis (call `services.AddQosRedisStore(...)` in this case) to create distributed counters. It is possible to mix local and distributed counters depending on the policy.

## Key parameter syntax

- If the key expression analyzer is setup (`services.AddExpressionPolicyKeyEvaluator()`), it is possible to write key C# expression as strings directly in the 'appsettings.json'.
    - Text enclosed between `@(` and `)` is considered as a C# expression.
    - Text enclosed between `@{` and `}` is considered as a C# method body (and must include a `return...` somewhere).
    - Both syntaxes can use a `context` variable that contains several properties and methods (see Github `AspNetCoreExt.Qos.ExpressionPolicyKeyEvaluator.Internal.Context.DefaultContext` source for details).
- The special string `"*"` means that the key is the same for every request. It is useful to setup a global rate limit policy of 100 calls per second, for example.
- It is possible to setup a custom policy by code: you have to write a class that implements `IQosPolicyKeyEvaluator`, and assign it in the options configuration.

## VIP (Very Important Person) middleware

Some administration requests can bypass the policies. A white list of special IP addresses can be set for that in a specific middleware.

## Policies in MVC

It is easy to link a policy directly to a MVC action.

The following code...

**Startup.cs**
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddQos();
    services.AddQosExpressionPolicyKeyEvaluator();
    services.Configure<QosQuotaOptions>(Configuration.GetSection("Quota"));
    services.AddQosQuota();

    services.AddMvc();
    services.AddQosMvc(); // Necessary to be able to use the QoS attribute
}
```

**MyController.cs**
```csharp
public class MyController : ControllerBase
{
    [HttpGet("/foo/bar1")]
    public string Get1() => "value1";

    [HttpGet("/foo/bar2")]
    [QosPolicy("MyPolicy")] // The special QoS attribute
    public string Get2() => "value2";
}
```

**appsettings.json**
```json
  "Quota": {
    "Policies": {
      "MyPolicy": {
        "UrlTemplates": [ "/foo/bar1" ],
        "Key": "@(context.Request.Url)",
        "MaxCount": 300,
        "Period": "0:0:30"
      }
    }
```

... is equivalent to...

**Startup.cs**
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddQos();
    services.AddQosExpressionPolicyKeyEvaluator();
    services.Configure<QosQuotaOptions>(Configuration.GetSection("Quota"));
    services.AddQosQuota();

    services.AddMvc();
}
```

**MyController.cs**
```csharp
public class MyController : ControllerBase
{
    [HttpGet("/foo/bar1")]
    public string Get1() => "value1";

    [HttpGet("/foo/bar2")]
    public string Get2() => "value2";
}
```

**appsettings.json**
```json
  "Quota": {
    "Policies": {
      "MyPolicy": {
        "UrlTemplates": [ "/foo/bar1", "GET /foo/bar2" ],
        "Key": "@(context.Request.Url)",
        "MaxCount": 300,
        "Period": "0:0:30"
      }
    }
```

You don't have to copy/paste the routes.

## Advanced customization

- Custom policy gates can be created by implementing the interface `IQosPolicyGate`.
- To accept specific syntax for the keys, implement `IQosPolicyKeyEvaluatorProvider`.
- To globally change/adjust parameters on policies, implement `IQosPolicyPostConfigure`.
- To create a totally new category of policies, see `IQosPolicyProvider`.
- To customize the error response, see the interface `IQosRejectResponse` or the default base class `DefaultQosRejectResponse`.
- To bypass policies according to different criteria of the IP address, create your own middleware that set a `IVipFeature` instance into the `HttpContext.Features` property before calling the QoS middleware.
- If you don't want to use Redis for your distributed counters, implement the interface `IQosDistributedCounterStore` in a new class.
