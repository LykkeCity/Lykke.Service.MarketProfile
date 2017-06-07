using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Lykke.MarketProfileService.Api
{
    class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel(options =>
                {
                    // TODO: Check all code for thread safety and remove it
                    options.ThreadCount = 1;
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseUrls("http://*:5000")
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();

            host.Run();
        }
    }
}