using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Redirect
{
    public class RedirectSettings
    {
        public string[] Froms { get; set; }
        public string To { get; set; }
        public Dictionary<Regex, string> QueryRules { get; set; }
        public Dictionary<string, Dictionary<Regex, string>> ContentRules { get; set; }
    }
}