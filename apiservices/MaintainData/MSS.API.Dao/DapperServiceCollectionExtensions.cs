using MSS.API.Model;
using MSS.API.Model.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using MSS.API.Dao.Implement;
using Dapper.FluentMap;
namespace MSS.API.Dao
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
            services.AddTransient<Itb_expert_dataRepo<tb_expert_data>, Tb_expert_dataRepo>();
            services.AddTransient<Itb_devicemaintain_regRepo<tb_devicemaintain_reg>, tb_devicemaintain_regRepo>(); 
            services.AddTransient<ILifeTimeKeyMaintainRepo<LifeTimeKeyMaintainInfo>, LifeTimeKeyMaintainRepo>(); 
            services.AddTransient<IEqpHistoryRepo<EqpHistory>, EqpHistoryRepo>(); 
            // 配置列名映射
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new BaseEntityMap());
                config.AddMap(new tb_expert_dataMap());
                config.AddMap(new tb_devicemaintain_regMap());
                config.AddMap(new EqpHistoryMap());
            });
            return services;
        }
    }
}
