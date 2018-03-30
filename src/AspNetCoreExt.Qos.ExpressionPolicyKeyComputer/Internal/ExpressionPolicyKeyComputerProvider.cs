namespace AspNetCoreExt.Qos.ExpressionPolicyKeyComputer.Internal
{
    public class ExpressionPolicyKeyComputerProvider : IQosPolicyKeyComputerProvider
    {
        public IQosPolicyKeyComputer TryCreate(object rawKeyComputer)
        {
            if (rawKeyComputer is string str && str.Length > 3)
            {
                if (str.StartsWith("@(") && str.EndsWith(")"))
                {
                    return new ExpressionPolicyKeyComputer(str.Substring(1));
                }
                else if (str.StartsWith("@{") && str.EndsWith("}"))
                {
                    return new ExpressionPolicyKeyComputer(str.Substring(1));
                }
            }

            return null;
        }
    }
}
