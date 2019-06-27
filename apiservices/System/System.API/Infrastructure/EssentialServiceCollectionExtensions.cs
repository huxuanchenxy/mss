using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.API.Service;
namespace System.API.Core.Infrastructure
{
    public static class EssentialServiceCollectionExtensions
    {
        public static IServiceCollection AddEssentialService(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            //BLL 统一注入 
           
            services.AddTransient<IConfigBigAreaService, ConfigBigAreaService>(); 
            services.AddTransient<IConfigMidAreaService, ConfigMidAreaService>();
            services.AddTransient<IService, BaseService>();
            return services;
        }
    }
}
