﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSS.Platform.Workflow.WebApi.Data;
using MSS.Platform.Workflow.WebApi.Service;
using MSS.API.Common;
using MSS.Platform.Workflow.WebApi.Infrastructure;
using MSS.Common.Consul;
using Microsoft.Extensions.Options;

namespace MSS.Platform.Workflow.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //PollingEngine.Configure(t => HostingEnvironment.QueueBackgroundWorkItem(aa => t()));
            

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "API", Version = "v1" });

            });
            services.AddDapper(Configuration);
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
            //PollingEngine.Configure(t => Task.Run(t));
            services.AddTransient<IWorkTaskService, WorkTaskService>();
            services.AddEssentialService();
            services.AddCSRedisCache(options =>
            {
                options.ConnectionString = this.Configuration["redis:ConnectionString"];
            });
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "MyAuthor API", Version = "v1" });
            //});
            services.AddConsulService(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime lifetime, IOptions<ConsulServiceEntity> consulService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection().UseCors(builder =>
builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "API");
            });
            app.RegisterConsul(lifetime, consulService);
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
