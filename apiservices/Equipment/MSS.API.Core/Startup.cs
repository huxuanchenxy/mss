﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MSS.API.Core.Infrastructure;
using MSS.API.Dao;
using MSS.API.Common;
using MSS.Common.Consul;
using MSS.API.Common.Global;

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

        readonly string AllowSpecificOrigins = "_AllowSpecificOrigins";

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
                                        "http://www.contoso.com")
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                });
                // options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });

            services.AddSingleton<GlobalActionFilter>();
            services.AddMvc(
                 options =>
                 {
                     options.Filters.Add<GlobalActionFilter>();
                 }
                ).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "MyAuthor API", Version = "v1" });
                //注入WebAPI注释文件给Swagger  
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, "MyAuthor.xml");
                //c.IncludeXmlComments(xmlPath);

                //c.IgnoreObsoleteActions();
                //////options.IgnoreObsoleteControllers();
                ////// 类、方法标记 [Obsolete]，可以阻止【Swagger文档】生成
                //c.DescribeAllEnumsAsStrings();
                ////c.OperationFilter<FormDataOperationFilter>();

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime, IOptions<ConsulServiceEntity> consulService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            app.RegisterConsul(lifetime, consulService);

            app.UseStaticFiles(new StaticFileOptions
            {
                //设置不限制content-type，即任何上传的文件都可以被下载
                ServeUnknownFileTypes = true
            });

            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            // app.UseCors(AllowSpecificOrigins);
            app.UseCors();
            app.UseMvc();
        }

        private void InitConst()
        {
            Const.PAGESIZE = Convert.ToInt32(Configuration["InitConst:PageSize"]);
            Const.LINE = Convert.ToInt32(Configuration["InitConst:Line"]);
            FilePath.BASEFILE = Configuration["InitConst:BaseFile"];
            FilePath.SHAREFILE = Configuration["InitConst:ShareFile"];
        }
    }
}
