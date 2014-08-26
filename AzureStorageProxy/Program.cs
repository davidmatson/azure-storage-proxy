using System;
using System.Configuration;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;

internal class Program
{
    private static void Main()
    {
        string accountName = ConfigurationManager.AppSettings["AccountName"];

        if (String.IsNullOrEmpty(accountName))
        {
            Console.Error.WriteLine("Storage credentials are required. Please provide AccountName in App.config.");
            return;
        }

        string accountKey = ConfigurationManager.AppSettings["AccountKey"];

        if (String.IsNullOrEmpty(accountKey))
        {
            Console.Error.WriteLine("Storage credentials are required. Please provide AccountKey in App.config.");
            return;
        }

        ProxyHandler.Initialize(accountName, accountKey);

        string listenUrl = "http://localhost:8080/";
        Console.WriteLine("WARNING: All traffic to Azure Storage will be unencrypted (HTTP, not HTTPS).");
        Console.WriteLine();

        using (WebApp.Start(listenUrl, Initialize))
        {
            Console.WriteLine("Listening on {0}", listenUrl);
            Console.WriteLine("Example request:");
            Console.WriteLine("GET {0}blob?comp=list HTTP/1.0", listenUrl);
            Console.WriteLine();
            Console.WriteLine("Press ENTER to exit . . .");

            while (Console.ReadKey(intercept: true).Key != ConsoleKey.Enter)
            {
            }
        }
    }

    private static void Initialize(IAppBuilder app)
    {
        HttpConfiguration configuration = new HttpConfiguration();

        configuration.MessageHandlers.Add(new ProxyHandler());

        app.UseWebApi(configuration);
    }
}
