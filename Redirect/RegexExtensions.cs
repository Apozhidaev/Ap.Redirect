using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Redirect
{
    internal static class RegexExtensions
    {
        public static string Replace(this IReadOnlyDictionary<Regex, string> rules, string input)
        {
            return rules.Aggregate(input, (current, pattern) => pattern.Key.Replace(current, pattern.Value));
        }
    }
}