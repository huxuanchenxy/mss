using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.API.DAO.Implement;
using System.API.DAO.Interface;
using System.API.Model; 

namespace System.API.DAO.Dapper
{
    public static class DapperServiceCollectionExtensions
    {
        public static IServiceCollection AddDapper(this IServiceCollection services, IConfiguration configuration)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            var optionsSection = configuration.GetSection("Dapper");
            var options = new DapperOptions();
            optionsSection.Bind(options);

            //数据访问层的注入可以在此处统一编写
             services.AddSingleton<DapperOptions>(options); 
            services.AddTransient<IConfigBigAreaRepo<TB_Config_BigArea>, ConfigBigAreaRepo>();
            services.AddTransient<IConfigMidAreaRepo<TB_Config_MidArea>, ConfigMidAreaRepo>();

        
            return services;
        }
    }

   
}
