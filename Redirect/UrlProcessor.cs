using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Redirect
{
    public class UrlProcessor : IUrlProcessor
    {
        private readonly Uri _baseUrl;
        private readonly Dictionary<Regex, string> _queryRules;

        public UrlProcessor(Uri baseUrl, Dictionary<Regex, string> queryRules = null)
        {
            _baseUrl = baseUrl;
            _queryRules = queryRules;
        }

        public Uri Process(Uri url)
        {
            var query = url.Query.TrimStart('?');
            if (!string.IsNullOrEmpty(query) && _queryRules != null)
            {
                var parameters = ParseQueryString(query);
                foreach (var parameter in parameters)
                {
                    if (!string.IsNullOrEmpty(parameter.Name))
                    {
                        parameter.Name = _queryRules.Replace(parameter.Name);
                    }
                    parameter.Value = _queryRules.Replace(parameter.Value);
                }
                query = parameters.Aggregate("", (current, parameter) => $"{current}&{parameter.ToString()}");
            }
            var uriBuilder = new UriBuilder(url)
            {
                Scheme = _baseUrl.Scheme,
                Host = _baseUrl.Host,
                Port = _baseUrl.Port,
                Query = query
            };
            return uriBuilder.Uri;
        }

        private static List<QueryParam> ParseQueryString(string query)
        {
            var list = new List<QueryParam>();
            if (string.IsNullOrEmpty(query)) return list;

            var l = query.Length;
            var i = 0;

            while (i < l)
            {
                var si = i;
                var ti = -1;
                while (i < l)
                {
                    var ch = query[i];
                    if (ch == '=')
                    {
                        if (ti < 0)
                            ti = i;
                    }
                    else if (ch == '&')
                    {
                        break;
                    }
                    i++;
                }

                var name = string.Empty;
                string value;
                if (ti >= 0)
                {
                    name = query.Substring(si, ti - si);
                    value = query.Substring(ti + 1, i - ti - 1);
                }
                else
                {
                    value = query.Substring(si, i - si);
                }
                list.Add(new QueryParam(WebUtility.UrlDecode(name), WebUtility.UrlDecode(value)));
                if (i == l - 1 && query[i] == '&')
                {
                    list.Add(new QueryParam(string.Empty, string.Empty));
                }
                i++;
            }
            return list;
        }

        private class QueryParam
        {
            public QueryParam(string name, string value)
            {
                Name = name;
                Value = value;
            }

            public string Name { get; set; }
            public string Value { get; set; }

            public override string ToString()
            {
                if (string.IsNullOrEmpty(Name))
                {
                    if (string.IsNullOrEmpty(Value)) return string.Empty;
                    return WebUtility.UrlEncode(Value) ?? string.Empty;
                }
                return $"{WebUtility.UrlEncode(Name)}={WebUtility.UrlEncode(Value)}";
            }
        }
    }
}