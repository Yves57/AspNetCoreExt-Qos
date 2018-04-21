using System.Text.RegularExpressions;

namespace AspNetCoreExt.Qos.Abstractions.Helpers
{
    public static class PolicyNameHelper
    {
        private static readonly Regex PolicyNameRegex = new Regex("^[a-z][a-z0-9_]*$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static bool IsValid(string name) => name != null && PolicyNameRegex.IsMatch(name);
    }
}
