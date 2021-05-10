using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace StockApi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "StockApi";
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureWebHostDefaults(webBuilder =>
                       {
                           webBuilder.UseKestrel(options =>
                           {
                               options.Limits.MaxConcurrentConnections = ushort.MaxValue;
                               options.Limits.MaxConcurrentUpgradedConnections = ushort.MaxValue;
                               options.Limits.MaxRequestLineSize = ushort.MaxValue;
                               options.Limits.MaxRequestBufferSize = int.MaxValue;
                               options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(15);
                               options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(120);
                           });
                           webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                           webBuilder.UseUrls("http://*:1000");
                           webBuilder.UseStartup<Startup>();
                       });
        }
    }
}