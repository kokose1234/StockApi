//  Copyright 2021 Jonguk Kim
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

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