using MSS.API.Operlog.Model;
using MSS.API.Operlog.Model.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Operlog.Dao.Interface;
using MSS.API.Operlog.Dao.Implement;

namespace MSS.API.Operlog.Dao
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
            services.AddSingleton<DapperOptions>(options);
            services.AddTransient<IUserOperationLogRepo<UserOperationLog>, UserOperationLogRepo>();
            
            return services;
        }
    }
}
