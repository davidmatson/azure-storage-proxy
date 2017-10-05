using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;

internal static class HttpRequestMessageExtensions
{
    public static bool IsAtLeastVersion(this HttpRequestMessage request, DateTime version)
    {
        if (request == null)
        {
            throw new ArgumentNullException("request");
        }

        if (version.TimeOfDay != TimeSpan.Zero)
        {
            throw new ArgumentException("version");
        }

        if (!request.Headers.Contains("x-ms-version"))
        {
            return false;
        }

        string requestVersion = request.Headers.GetValues("x-ms-version").First();
        string compareToText = version.ToString("yyyy'-'MM'-'dd", CultureInfo.InvariantCulture);

        return compareToText.CompareTo(requestVersion) <= 0;
    }
}
