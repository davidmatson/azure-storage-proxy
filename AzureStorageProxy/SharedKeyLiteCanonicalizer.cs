using System.Net.Http;
using System.Text;

internal class SharedKeyLiteCanonicalizer : IRequestCanonicalizer
{
    private static SharedKeyLiteCanonicalizer _instance =
        new SharedKeyLiteCanonicalizer(SharedKeyLiteResourceCanonicalizer.Instance);

    private readonly IResourceCanonicalizer _resourceCanonicalizer;

    private SharedKeyLiteCanonicalizer(IResourceCanonicalizer resourceCanonicalizer)
    {
        _resourceCanonicalizer = resourceCanonicalizer;
    }

    public static SharedKeyLiteCanonicalizer Instance
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
            output.Append(request.Content.Headers.ContentMD5);
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
        HeaderCanonicalizer.CanonicalizeHeaders(request, output);
        _resourceCanonicalizer.CanonicalizeResource(accountName, request, output);
        return output.ToString();
    }
}
