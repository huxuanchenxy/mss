﻿using MSS.API.Core.V1.Business;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.AspNetCore.Http;
using MSS.API.Common.Utility;
using MSS.Common.Consul;

namespace MSS.API.Core.Infrastructure
{
    public static class EssentialServiceCollectionExtensions
    {
        public static IServiceCollection AddEssentialService(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddTransient<IOrgService, OrgService>();
            services.AddTransient<IWarnningSettingService, WarnningSettingService>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IAuthHelper, AuthHelper>();
            services.AddTransient<IServiceDiscoveryProvider, ConsulServiceProvider>();
            services.AddTransient<IWarnningService, WarnningService>();
            services.AddTransient<IMetroLineService, MetroLineService>();
            return services;
        }
    }
}
