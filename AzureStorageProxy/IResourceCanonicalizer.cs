using System.Net.Http;
using System.Text;

internal interface IResourceCanonicalizer
{
    void CanonicalizeResource(string accountName, HttpRequestMessage request, StringBuilder output);
}
