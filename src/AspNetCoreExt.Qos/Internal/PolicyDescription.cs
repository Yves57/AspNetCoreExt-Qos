using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCoreExt.Qos.Internal
{
    public class PolicyDescription
    {
        private static readonly string AllUrlWildcard = "*";

        private readonly TemplateMatcher[] _templateMatchers;

        public PolicyDescription(QosPolicy policy)
        {
            Policy = policy;
            _templateMatchers = BuilderTemplateMatchers().ToArray();
        }

        public QosPolicy Policy { get; }

        private IEnumerable<TemplateMatcher> BuilderTemplateMatchers()
        {
            if (Policy.UrlTemplates != null)
            {
                foreach (var urlTemplate in Policy.UrlTemplates)
                {
                    var template = urlTemplate;
                    if (template.StartsWith("/"))
                    {
                        template = template.Substring(1);
                    }
                    else if (template.StartsWith("~/"))
                    {
                        template = template.Substring(2);
                    }

                    var routeTemplate = TemplateParser.Parse(template);

                    yield return new TemplateMatcher(routeTemplate, null);
                }
            }
        }

        public bool TryUrlMatching(PathString url, RouteValueDictionary routeValues, out RouteTemplate routeTemplate)
        {
            foreach (var matcher in _templateMatchers)
            {
                if (matcher.Template.TemplateText == AllUrlWildcard ||
                    matcher.TryMatch(url, routeValues))
                {
                    routeTemplate = matcher.Template;
                    return true;
                }
            }

            routeTemplate = null;
            return false;
        }
    }
}
