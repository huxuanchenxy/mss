using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using Quartz;
using Quartz.Impl;
using MSS.API.Core.Job;
using Quartz.Spi;

namespace MSS.API.Core
{

    public class Program
    {
        //public static void Main(string[] args)
        //{
        //    CreateWebHostBuilder(args).Build().Run();
        //}

        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //        .UseStartup<Startup>();

        public static void Main(string[] args)
        {
            //RunJob().GetAwaiter().GetResult();
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            int port = int.Parse(args[0]);//3861
            return WebHost.CreateDefaultBuilder(args)

                .UseContentRoot(Directory.GetCurrentDirectory())
                 .UseIISIntegration()

                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Any, port);
                    //options.Listen(IPAddress.Any, 443, listenOptions =>
                    //{
                    //    listenOptions.UseHttps("server.pfx", "password");
                    //});
                })
              .UseStartup<Startup>()
                .Build();
        }
    }
}
