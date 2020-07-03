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
using Serilog;
using Serilog.Events;

namespace MSS.API.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            int port = int.Parse(args[0]);//3861
            return WebHost.CreateDefaultBuilder(args)

                //.UseContentRoot(Directory.GetCurrentDirectory())
                .UseContentRoot(Path.GetDirectoryName(typeof(Program).Assembly.Location))
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Any, port);
                    //options.Listen(IPAddress.Any, 443, listenOptions =>
                    //{
                    //    listenOptions.UseHttps("server.pfx", "password");
                    //});
                })
                .UseStartup<Startup>()
                .UseSerilog((context, configuration) => configuration
                .Enrich.FromLogContext()
                .MinimumLevel.Information() //这个根据level来，比如到error那information级别的就不出来了
                //.WriteTo.Console()
                //.WriteTo.File(Path.Combine("Logs", @"log.txt"), rollingInterval: RollingInterval.Day)
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.RollingFile(@"EventServiceLog/Information-{Date}.log"))
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning).WriteTo.RollingFile(@"EventServiceLog/Warning-{Date}.log"))
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo.RollingFile(@"EventServiceLog/Debug-{Date}.log"))
                .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.RollingFile(@"EventServiceLog/Error-{Date}.log")),
                preserveStaticLogger: true)
                .Build();
        }
    }
}
