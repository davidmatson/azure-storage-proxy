using System.Net.Http;

internal interface IRequestCanonicalizer
{
    string Canonicalize(string accountName, HttpRequestMessage request);
}
