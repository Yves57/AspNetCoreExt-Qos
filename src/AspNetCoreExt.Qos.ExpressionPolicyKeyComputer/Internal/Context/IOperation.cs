namespace AspNetCoreExt.Qos.ExpressionPolicyKeyComputer.Internal.Context
{
    public interface IOperation
    {
        string Method { get; }

        string UrlTemplate { get; }
    }
}
