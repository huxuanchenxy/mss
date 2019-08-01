using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MSS.API.Core.Infrastructure;
using MSS.API.Dao;
using MSS.Common.Consul;
using MSS.API.Common;
using MSS.API.Core.EventServer;

using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using Microsoft.AspNetCore.SignalR;
namespace MSS.API.Core
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // readonly string AllowSpecificOrigins = "_AllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //压缩 gzip
            services.AddResponseCompression();

            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

            services.AddDapper(Configuration);
            services.AddCSRedisCache(options =>
            {
                options.ConnectionString = this.Configuration["redis:ConnectionString"];
            });
            services.AddEssentialService();
            services.AddConsulService(Configuration);
            //跨域 Cors
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                builder =>
                {

                    builder.WithOrigins("http://localhost:8080",
                                        "http://10.89.36.103")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod()
                                        .AllowCredentials();
                });
                // options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });

            services.AddSignalR();
            services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
            // 注册quarz服务
            services.AddHostedService<ScheduleService>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();//注册ISchedulerFactory的实例。
            services.AddSingleton<IJobFactory, MssJobFactory>();
            // 报警队列监控任务
            services.AddTransient<MsgQueueWatcher>();
            // 获取所有pid任务
            services.AddTransient<InitConfigJob>();
            // 预警分析任务
            services.AddTransient<WarningJob>();
            // 设备维修通知任务
            services.AddTransient<NotificationJob>();
            
            // 报警队列
            services.AddSingleton<EventQueues>();
            // 更新全局数据任务
            services.AddSingleton<GlobalDataManager>();
            

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime, IOptions<ConsulServiceEntity> consulService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            // app.UseCors(AllowSpecificOrigins);
            app.RegisterConsul(lifetime, consulService);
            app.UseCors();
            app.UseSignalR(routes =>
            {
                routes.MapHub<MssEventHub>("/eventHub");
            });
            app.UseMvc();
        }
    }
}
