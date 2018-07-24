namespace AspNetCoreExt.Qos.ExpressionPolicyKeyEvaluator.Internal
{
    public class ExpressionPolicyKeyEvaluatorProvider : IQosPolicyKeyEvaluatorProvider
    {
        public IQosPolicyKeyEvaluator TryCreate(object rawKeyEvaluator)
        {
            if (rawKeyEvaluator is string str && str.Length > 3)
            {
                if (str.StartsWith("@(") && str.EndsWith(")"))
                {
                    return new ExpressionPolicyKeyEvaluator(str.Substring(1));
                }
                else if (str.StartsWith("@{") && str.EndsWith("}"))
                {
                    return new ExpressionPolicyKeyEvaluator(str.Substring(1));
                }
            }

            return null;
        }
    }
}
