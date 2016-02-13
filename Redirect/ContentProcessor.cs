using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace Redirect
{
    public class ContentProcessor : IContentProcessor
    {
        private readonly Dictionary<string, Dictionary<Regex, string>> _rules;

        public ContentProcessor(Dictionary<string, Dictionary<Regex, string>> rules)
        {
            _rules = rules;
        }

        public byte[] Process(byte[] bytes, MediaTypeHeaderValue type, Uri url)
        {
            if (type != null && _rules.ContainsKey(type.MediaType))
            {
                var rule = _rules[type.MediaType];
                var encoding = GetEncoding(type.CharSet);
                var content = encoding.GetString(bytes);
                content = rule.Replace(content);
                bytes = encoding.GetBytes(content);
            }
            return bytes;
        }

        private static Encoding GetEncoding(string charSet)
        {
            if (!string.IsNullOrEmpty(charSet))
            {
                if (charSet.Equals("cp1251", StringComparison.InvariantCultureIgnoreCase))
                {
                    charSet = "windows-1251";
                }
                return Encoding.GetEncoding(charSet);
            }
            return Encoding.UTF8;
        }
    }
}