using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace AspNetCoreExt.Qos.ExpressionPolicyKeyComputer.Internal.Context
{
    public interface IRequest
    {
        X509Certificate2 Certificate { get; }

        IRequestHeaders Headers { get; }

        string IpAddress { get; }

        string Method { get; }

        string Url { get; }

        string RouteTemplate { get; }

        IDictionary<string, string> RouteValues { get; }

        ClaimsPrincipal User { get; }
    }
}
