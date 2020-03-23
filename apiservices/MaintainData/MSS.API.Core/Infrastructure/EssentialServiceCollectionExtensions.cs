using MSS.API.Core.V1.Business;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Common.Utility;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;

namespace MSS.API.Core.Infrastructure
{
    public static class EssentialServiceCollectionExtensions
    {
        public static IServiceCollection AddEssentialService(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services)); 
            services.AddTransient<IExpertDataService, ExpertDataService>();
            services.AddTransient<IDeviceMaintainRegService, DeviceMaintainRegService>();
            services.AddTransient<ILifeTimeKeyMaintainService, LifeTimeKeyMaintainService>();
            services.AddTransient<IEqpHistoryService, EqpHistoryService>();
            services.AddTransient<IEquipmentRepairHistoryService, EquipmentRepairHistoryService>();
            services.AddTransient<IWorkingApplicationService, WorkingApplicationService>();
            services.AddTransient<ITroubleReportService, TroubleReportService>();
            services.AddTransient<IEmergencyPlanService, EmergencyPlanService>();
            services.AddTransient<IHealthConfigService, HealthConfigService>();
            services.AddTransient<IHealthService, HealthService>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IAuthHelper, AuthHelper>();
            return services;
        }
    }
}
