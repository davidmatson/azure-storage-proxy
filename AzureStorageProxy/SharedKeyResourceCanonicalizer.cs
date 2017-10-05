using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

internal class SharedKeyResourceCanonicalizer : IResourceCanonicalizer
{
    private static SharedKeyResourceCanonicalizer _instance = new SharedKeyResourceCanonicalizer();

    private SharedKeyResourceCanonicalizer()
    {
    }

    public static SharedKeyResourceCanonicalizer Instance
    {
        get { return _instance; }
    }

    public void CanonicalizeResource(string accountName, HttpRequestMessage request, StringBuilder output)
    {
        output.Append('/');
        output.Append(accountName);
        UriBuilder uriBuilder = new UriBuilder(request.RequestUri);
        output.Append(uriBuilder.Path);
        CanonicalizeQuery(uriBuilder.Query, output);
    }

    private static void CanonicalizeQuery(string query, StringBuilder output)
    {
        var parameters = QueryStringParser.Parse(query).Select(
            p => new KeyValuePair<string, string>(p.Key.ToLowerInvariant(), p.Value)).OrderBy(p => p.Key);

        foreach (var group in parameters.GroupBy(p => p.Key))
        {
            output.Append('\n');

            string key = HttpUtility.UrlDecode(group.Key);
            output.Append(key);
            output.Append(':');
            bool first = true;

            foreach (var value in group.Select(p => p.Value).OrderBy(v => v))
            {
                if (!first)
                {
                    output.Append(',');
                }

                string decodedValue = HttpUtility.UrlDecode(value);
                output.Append(decodedValue);
                first = false;
            }
        }
    }
}
