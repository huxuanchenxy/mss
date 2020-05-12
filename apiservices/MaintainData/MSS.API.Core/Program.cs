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
using Serilog;
using Serilog.Events;

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

                //.UseContentRoot(Directory.GetCurrentDirectory())
                .UseContentRoot(Path.GetDirectoryName(typeof(Program).Assembly.Location))
                 //.UseIISIntegration()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Any, port);
                    //options.Listen(IPAddress.Any, 443, listenOptions =>
                    //{
                    //    listenOptions.UseHttps("server.pfx", "password");
                    //});
                })
              .UseStartup<Startup>().UseSerilog((context, configuration) => configuration
                .Enrich.FromLogContext()
                .MinimumLevel.Warning()
                .WriteTo.Console()
                //.WriteTo.File(Path.Combine("Logs", @"log.txt"), rollingInterval: RollingInterval.Day)
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.RollingFile(@"Logs/Information-{Date}.log"))
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning).WriteTo.RollingFile(@"Logs/Warning-{Date}.log"))
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo.RollingFile(@"Logs/Debug-{Date}.log"))
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.RollingFile(@"Logs/Error-{Date}.log")),

                preserveStaticLogger: true)
                .Build();
        }
    }
}
