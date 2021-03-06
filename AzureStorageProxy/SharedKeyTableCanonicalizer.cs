﻿using System;
using System.Linq;
using System.Net.Http;
using System.Text;

internal class SharedKeyTableCanonicalizer : IRequestCanonicalizer
{
    private static SharedKeyTableCanonicalizer _instance =
        new SharedKeyTableCanonicalizer(SharedKeyLiteResourceCanonicalizer.Instance);

    private readonly IResourceCanonicalizer _resourceCanonicalizer;

    private SharedKeyTableCanonicalizer(IResourceCanonicalizer resourceCanonicalizer)
    {
        _resourceCanonicalizer = resourceCanonicalizer;
    }

    public static SharedKeyTableCanonicalizer Instance
    {
        get { return _instance; }
    }

    public string Canonicalize(string accountName, HttpRequestMessage request)
    {
        StringBuilder output = new StringBuilder();
        output.Append(request.Method.Method.ToUpperInvariant());
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
        else if (request.Headers.Contains("x-ms-date"))
        {
            output.Append(request.Headers.GetValues("x-ms-date").First());
        }
        output.Append('\n');
        _resourceCanonicalizer.CanonicalizeResource(accountName, request, output);

        return output.ToString();
    }
}
