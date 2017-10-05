using System;
using System.Linq;
using System.Net.Http;
using System.Text;

internal class SharedKeyCanonicalizer : IRequestCanonicalizer
{
    private static SharedKeyCanonicalizer _instance =
        new SharedKeyCanonicalizer(SharedKeyResourceCanonicalizer.Instance);

    private readonly IResourceCanonicalizer _resourceCanonicalizer;

    private SharedKeyCanonicalizer(IResourceCanonicalizer resourceCanonicalizer)
    {
        _resourceCanonicalizer = resourceCanonicalizer;
    }

    public static SharedKeyCanonicalizer Instance
    {
        get { return _instance; }
    }

    public string Canonicalize(string accountName, HttpRequestMessage request)
    {
        StringBuilder output = new StringBuilder();
        output.Append(request.Method.Method.ToUpperInvariant());
        output.Append('\n');
        if (request.Content != null)
        {
            output.Append(String.Join(" ", request.Content.Headers.ContentEncoding));
        }
        output.Append('\n');
        if (request.Content != null)
        {
            output.Append(String.Join(" ", request.Content.Headers.ContentLanguage));
        }
        output.Append('\n');
        if (request.Content != null && request.Content.Headers.ContentLength.HasValue)
        {
            long contentLength = request.Content.Headers.ContentLength.Value;

            if (contentLength > 0 || !request.IsAtLeastVersion(new DateTime(2015, 02, 21)))
            {
                output.Append(contentLength);
            }
        }
        output.Append('\n');
        if (request.Content != null && request.Content.Headers.ContentMD5 != null)
        {
            output.Append(Convert.ToBase64String(request.Content.Headers.ContentMD5));
        }
        output.Append('\n');
        if (request.Content != null)
        {
            output.Append(request.Content.Headers.ContentType);
        }
        output.Append('\n');
        if (request.Headers.Date.HasValue)
        {
            output.Append(request.Headers.Date.Value.ToString("r"));
        }
        output.Append('\n');
        if (request.Headers.IfModifiedSince.HasValue)
        {
            output.Append(request.Headers.IfModifiedSince.Value.ToString("r"));
        }
        output.Append('\n');
        output.Append(String.Join(" ", request.Headers.IfMatch.Select(h => h.ToString())));
        output.Append('\n');
        output.Append(String.Join(" ", request.Headers.IfNoneMatch.Select(h => h.ToString())));
        output.Append('\n');
        if (request.Headers.IfUnmodifiedSince.HasValue)
        {
            output.Append(request.Headers.IfUnmodifiedSince.Value.ToString("r"));
        }
        output.Append('\n');
        if (request.Headers.Range != null)
        {
            output.Append(request.Headers.Range.ToString());
        }
        output.Append('\n');
        HeaderCanonicalizer.CanonicalizeHeaders(request, output);
        _resourceCanonicalizer.CanonicalizeResource(accountName, request, output);
        return output.ToString();
    }
}
