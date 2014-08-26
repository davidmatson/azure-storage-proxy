using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;

internal class SharedKeyLiteResourceCanonicalizer : IResourceCanonicalizer
{
    private static SharedKeyLiteResourceCanonicalizer _instance = new SharedKeyLiteResourceCanonicalizer();

    private SharedKeyLiteResourceCanonicalizer()
    {
    }

    public static SharedKeyLiteResourceCanonicalizer Instance
    {
        get { return _instance; }
    }

    public void CanonicalizeResource(string accountName, HttpRequestMessage request, StringBuilder output)
    {
        output.Append('/');
        output.Append(accountName);
        UriBuilder uriBuilder = new UriBuilder(request.RequestUri);
        output.Append(uriBuilder.Path);
        output.Append(GetCanonicalizedQuery(uriBuilder.Query));
    }

    private static string GetCanonicalizedQuery(string query)
    {
        var parts = QueryStringParser.Parse(query);

        KeyValuePair<string, string> comp = parts.SingleOrDefault(p => p.Key.ToLowerInvariant() == "comp");

        if (comp.Key == null)
        {
            return null;
        }
        else
        {
            return String.Format(CultureInfo.InvariantCulture, "?{0}={1}", comp.Key, comp.Value);
        }
    }
}
