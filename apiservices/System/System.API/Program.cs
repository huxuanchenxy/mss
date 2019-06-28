﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace System.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();


        //public static void Main(string[] args)
        //{
        //    BuildWebHost(args).Run();
        //}

        //public static IWebHost BuildWebHost(string[] args)
        //{
        //    int port = int.Parse(args[0]);//3861
        //    return WebHost.CreateDefaultBuilder(args)

        //        .UseContentRoot(Directory.GetCurrentDirectory())
        //         .UseIISIntegration()

        //        .UseKestrel(options =>
        //        {
        //            options.Listen(IPAddress.Any, port);
        //            //options.Listen(IPAddress.Any, 443, listenOptions =>
        //            //{
        //            //    listenOptions.UseHttps("server.pfx", "password");
        //            //});
        //        })
        //      .UseStartup<Startup>()
        //        .Build();
        //}

    }
}
