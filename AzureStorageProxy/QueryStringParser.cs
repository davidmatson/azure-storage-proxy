using System;
using System.Collections.Generic;
using System.Linq;

internal static class QueryStringParser
{
    public static IEnumerable<KeyValuePair<string, string>> Parse(string queryString)
    {
        if (String.IsNullOrEmpty(queryString))
        {
            return Enumerable.Empty<KeyValuePair<string, string>>();
        }

        if (queryString[0] != '?')
        {
            throw new ArgumentException("queryString", "A query string must start with a question mark character.");
        }

        List<KeyValuePair<string, string>> parts = new List<KeyValuePair<string, string>>();

        string[] pairs = queryString.Substring(1).Split('&');

        foreach (string pair in pairs)
        {
            int equalsIndex = pair.IndexOf('=');

            if (equalsIndex == -1)
            {
                parts.Add(new KeyValuePair<string, string>(pair, String.Empty));
            }
            else
            {
                parts.Add(new KeyValuePair<string, string>(
                    pair.Substring(0, equalsIndex), pair.Substring(equalsIndex + 1)));
            }
        }

        return parts;
    }
}
