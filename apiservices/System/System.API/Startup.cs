using System;
using System.API.Core.Infrastructure;
using System.API.DAO.Dapper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;

using MSS.Common.Consul;
using MSS.API.Common;

namespace System.API
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)  //(IConfiguration configuration)
        {
            // Configuration = configuration;
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //压缩 gzip
            services.AddResponseCompression();

            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

            services.AddDapper(Configuration);
            services.AddEssentialService();
            services.AddConsulService(Configuration);

            //services.AddAuthentication("Bearer")//添加授权模式
            //.AddIdentityServerAuthentication(Options =>
            //{
            //    Options.Authority = "http://localhost:5000";//授权服务器地址
            //    Options.RequireHttpsMetadata = false;//是否是https
            //    //Options.JwtValidationClockSkew = TimeSpan.FromSeconds(0);//设置时间偏移
            //    Options.ApiName = "MSS_WEBAPI";
            //});
            //跨域 Cors
            //services.AddCors(options =>
            //{
            //    options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            //});

            //跨域 Cors
            services.AddCors(options =>
            {
                //options.AddDefaultPolicy(
                //builder =>
                //{

                //    builder.WithOrigins("http://localhost:8080",
                //                        "http://www.contoso.com")
                //                        .AllowAnyHeader()
                //                        .AllowAnyMethod();
                //});
                options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
            });
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "MySystem API", Version = "v1" });
                // 为 Swagger JSON and UI设置xml文档注释路径
                //var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
                //var xmlPath = Path.Combine(basePath, "MySystem.xml");//和项目名对应
                //c.IncludeXmlComments(xmlPath);
            });

        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime,
            IOptions<ConsulServiceEntity> consulService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();
            //else
            //{
            //    app.UseHsts();
            //}
            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(c =>
            {
                //c.InjectOnCompleteJavaScript("/swagger/ui/zh_CN.js"); // 加载中文包
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MySystem V1");
            });
            app.UseHttpsRedirection();
            app.UseCors("AllowAll");
            app.RegisterConsul(lifetime, consulService);
            app.UseMvc();
        }
    }
}