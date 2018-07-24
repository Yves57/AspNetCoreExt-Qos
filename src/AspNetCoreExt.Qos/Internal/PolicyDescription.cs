using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCoreExt.Qos.Internal
{
    public class PolicyDescription
    {
        private static readonly string AllUrlWildcard = "*";

        private struct Template
        {
            public string HttpMethod;

            public TemplateMatcher Matcher;
        }

        private readonly Template[] _templates;

        public PolicyDescription(QosPolicy policy)
        {
            Policy = policy;
            _templates = BuilderTemplates().ToArray();
        }

        public QosPolicy Policy { get; }

        private IEnumerable<Template> BuilderTemplates()
        {
            if (Policy.UrlTemplates != null)
            {
                foreach (var urlTemplate in Policy.UrlTemplates)
                {
                    var url = urlTemplate.Url;
                    if (url.StartsWith("/"))
                    {
                        url = url.Substring(1);
                    }
                    else if (url.StartsWith("~/"))
                    {
                        url = url.Substring(2);
                    }

                    var routeTemplate = TemplateParser.Parse(url);

                    yield return new Template()
                    {
                        HttpMethod = urlTemplate.HttpMethod,
                        Matcher = new TemplateMatcher(routeTemplate, null)
                    };
                }
            }
        }

        public bool TryUrlMatching(string method, PathString url, RouteValueDictionary routeValues, out RouteTemplate routeTemplate)
        {
            foreach (var template in _templates)
            {
                if (template.HttpMethod == null || template.HttpMethod.Equals(method, StringComparison.OrdinalIgnoreCase))
                {
                    if (template.Matcher.Template.TemplateText == AllUrlWildcard ||
                        template.Matcher.TryMatch(url, routeValues))
                    {
                        routeTemplate = template.Matcher.Template;
                        return true;
                    }
                }
            }

            routeTemplate = null;
            return false;
        }
    }
}
