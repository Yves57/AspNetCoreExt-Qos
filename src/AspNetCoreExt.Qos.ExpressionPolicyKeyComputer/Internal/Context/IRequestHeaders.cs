using System.Collections.Generic;

namespace AspNetCoreExt.Qos.ExpressionPolicyKeyComputer.Internal.Context
{
    public interface IRequestHeaders : IReadOnlyDictionary<string, string[]>
    {
        string GetValueOrDefault(string headerName, string defaultValue);
    }
}
