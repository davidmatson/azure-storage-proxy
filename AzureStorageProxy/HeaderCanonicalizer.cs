using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

internal static class HeaderCanonicalizer
{
    public static void CanonicalizeHeaders(HttpRequestMessage request, StringBuilder output)
    {
        foreach (var header in request.Headers
            .Where(h => h.Key.StartsWith("x-ms-"))
            .Select(h => new KeyValuePair<string, string>(h.Key.ToLowerInvariant(), h.Value.First()))
            .OrderBy(h => h.Key))
        {
            output.Append(header.Key);
            output.Append(':');
            output.Append(header.Value);
            output.Append('\n');
        }
    }
}
