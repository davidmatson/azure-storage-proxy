using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

internal sealed class ProxyHandler : DelegatingHandler
{
    private static string _accountName;
    private static HMACSHA256 _hmac;

    private readonly HttpMessageInvoker _invoker = new HttpClient();

    private bool _disposed;

    public static void Initialize(string accountName, string accountKey)
    {
        _accountName = accountName;
        _hmac = new HMACSHA256(Convert.FromBase64String(accountKey));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        UriBuilder builder = new UriBuilder(request.RequestUri);
        string storageType;
        string path;
        SplitPath(builder.Path, out storageType, out path);
        builder.Path = path;
        builder.Host = String.Format(CultureInfo.InvariantCulture,
            "{0}.{1}.core.windows.net", _accountName, storageType);
        builder.Port = 80;
        request.RequestUri = builder.Uri;
        request.Headers.Host = builder.Host;

        if (request.Method == HttpMethod.Get || request.Method == HttpMethod.Head)
        {
            request.Content = null;
        }

        if (!request.Headers.Date.HasValue && !request.Headers.Contains("x-ms-date"))
        {
            request.Headers.Date = DateTimeOffset.Now;
        }

        if (request.Headers.Authorization == null)
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("SharedKey", GetSignature(request, forTable: storageType == "table"));
        }

        HttpResponseMessage response = await _invoker.SendAsync(request, cancellationToken);
        return response;
    }

    private static void SplitPath(string path, out string firstPart, out string remainder)
    {
        if (string.IsNullOrEmpty(path))
        {
            firstPart = path;
            remainder = null;
            return;
        }

        if (path[0] == '/')
        {
            path = path.Substring(1);
        }

        int slashIndex = path.IndexOf('/');

        if (slashIndex == -1)
        {
            firstPart = path;
            remainder = null;
            return;
        }

        firstPart = path.Substring(0, slashIndex);
        remainder = path.Substring(slashIndex + 1);
    }

    private static string GetSignature(HttpRequestMessage request, bool forTable)
    {
        return _accountName + ":" + Convert.ToBase64String(_hmac.ComputeHash(
            Encoding.UTF8.GetBytes(GetStringToSign(request, forTable))));
    }

    private static string GetStringToSign(HttpRequestMessage request, bool forTable)
    {
        if (!forTable)
        {
            if (request.Headers.Contains("x-ms-version"))
            {
                string firstVersion = request.Headers.GetValues("x-ms-version").First();

                if ("2009-09-19".CompareTo(firstVersion) <= 0)
                {
                    return SharedKeyCanonicalizer.Instance.Canonicalize(_accountName, request);
                }
            }

            return SharedKeyLiteCanonicalizer.Instance.Canonicalize(_accountName, request);
        }
        else
        {
            return SharedKeyTableCanonicalizer.Instance.Canonicalize(_accountName, request);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _invoker.Dispose();
            _disposed = true;
        }

        base.Dispose(disposing);
    }
}
