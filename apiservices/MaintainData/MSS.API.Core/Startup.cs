﻿using System;
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
using System.IO; 
using Microsoft.AspNetCore.HttpsPolicy; 
using Swashbuckle.AspNetCore.Swagger;
using MSS.Common.Consul;
using MSS.API.Common;
using MSS.API.Common.Global;
using Quartz;
using Quartz.Impl;
using MSS.API.Core.Job;
using Quartz.Spi;
using Serilog;
using Serilog.Events;

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
            InitConst();
        }

        //readonly string AllowSpecificOrigins = "_AllowSpecificOrigins";

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
            //services.AddConsulService(Configuration);
            services.AddConsulService(Configuration);
            //跨域 Cors
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                builder =>
                {

                    builder.WithOrigins("http://localhost:8080",
                                        "http://www.contoso.com")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                });
                //options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });
            services.AddSingleton<GlobalActionFilter>();
            services.AddMvc(
                options =>
                {
                    options.Filters.Add<GlobalActionFilter>();
                }
            ).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "ExpertData API", Version = "v1" });
                // 为 Swagger JSON and UI设置xml文档注释路径
                //var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                //var xmlPath = Path.Combine(basePath, "ExpertData.xml");//和项目名对应
                //c.IncludeXmlComments(xmlPath);
            });
            //Quartz
            services.AddSingleton<HealthJob>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();//注册ISchedulerFactory的实例。
            services.AddSingleton<QuartzStart>();
            services.AddSingleton<IJobFactory, IOCJobFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime, IOptions<ConsulServiceEntity> consulService, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.RegisterConsul(lifetime, consulService);

            // app.UseCors(AllowSpecificOrigins); 
            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                //c.InjectOnCompleteJavaScript("/swagger/ui/zh_CN.js"); // 加载中文包
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MySystem V1");
            });
            //app.UseHttpsRedirection();
            
            app.UseCors();
            app.UseMvc();
            var quartz = app.ApplicationServices.GetRequiredService<QuartzStart>();
            lifetime.ApplicationStarted.Register(()=> {
                quartz.Start().Wait();
            });
            lifetime.ApplicationStopped.Register(() => {
                quartz.Stop();
            });
        }

        private void InitConst()
        {
            Const.LINE = Convert.ToInt32(Configuration["InitConst:Line"]);
            FilePath.BASEFILE = Configuration["InitConst:BaseFile"];
            FilePath.SHAREFILE = Configuration["InitConst:ShareFile"];
        }
    }
}
