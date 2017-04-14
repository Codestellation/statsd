using System;
using System.Collections.Generic;

namespace Codestellation.Statsd.Builder
{
    internal static class UriParseExtensions
    {
        public const string Background = "background";
        public const string IgnoreExceptions = "ignore_exceptions";
        public const string Prefix = "prefix";

        public static Dictionary<string, string> GetQueryValues(this Uri self)
        {
            if (self == null)
            {
                throw new ArgumentNullException(nameof(self));
            }
            var queryValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrWhiteSpace(self.Query))
            {
                string query = self.Query.Replace("?", string.Empty);
                string[] kvp = query.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string pair in kvp)
                {
                    var tokens = pair.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    var key = tokens[0];
                    //we may not have value for background and ignore_exception options.
                    var value = tokens.Length > 1 ? tokens[1] : string.Empty;

                    queryValues.Add(key, value);
                }
            }

            return queryValues;
        }

        public static bool ParseOrDefault(this IDictionary<string, string> self, string key, bool onDefault = false)
        {
            string candidate;
            bool result;
            if (self.TryGetValue(key, out candidate) && !string.IsNullOrWhiteSpace(candidate) && bool.TryParse(candidate, out result))
            {
                return result;
            }
            return onDefault;
        }

        public static string ParseOrDefault(this IDictionary<string, string> self, string key, string onDefault = null)
        {
            string candidate;
            if (self.TryGetValue(key, out candidate) && !string.IsNullOrWhiteSpace(candidate))
            {
                return candidate;
            }
            return onDefault;
        }
    }
}