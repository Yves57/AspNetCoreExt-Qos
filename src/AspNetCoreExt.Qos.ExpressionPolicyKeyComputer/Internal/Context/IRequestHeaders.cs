using System.Collections.Generic;

namespace AspNetCoreExt.Qos.ExpressionPolicyKeyEvaluator.Internal.Context
{
    public interface IRequestHeaders : IReadOnlyDictionary<string, string[]>
    {
        string GetValueOrDefault(string headerName, string defaultValue);
    }
}
