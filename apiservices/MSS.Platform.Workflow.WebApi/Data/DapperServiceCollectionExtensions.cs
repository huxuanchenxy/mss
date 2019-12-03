using Dapper.FluentMap;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MSS.Platform.Workflow.WebApi.Model;
using System;

namespace MSS.Platform.Workflow.WebApi.Data
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

            services.AddTransient<IConstructionPlanRepo<ConstructionPlan>, ConstructionPlanRepo>();
            services.AddTransient<IConstructionPlanImportRepo<ConstructionPlanYear>, ConstructionPlanImportRepo>();
            services.AddTransient<IConstructionPlanMonthDetailRepo<ConstructionPlanMonthDetail>, ConstructionPlanMonthDetailRepo>();
            services.AddTransient<IWorkTaskRepo<TaskViewModel>, WorkTaskRepo>();
            services.AddTransient<IWfprocessRepo<Wfprocess>, WfprocessRepo>();
            services.AddTransient<IConstructionPlanMonthChartRepo<ConstructionPlanMonthChart>, ConstructionPlanMonthChartRepo>();

            //配置列名映射
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new ConstructionPlanMap());
                config.AddMap(new ConstructionPlanYearMap());
                config.AddMap(new ConstructionPlanMonthMap());
                config.AddMap(new ConstructionPlanMonthDetailMap());
                config.AddMap(new ConstructionPlanImportCommonMap());
                config.AddMap(new WfprocessMap());
                config.AddMap(new ConstructionPlanMonthChartMap());
            });
            return services;
        }
    }
}
