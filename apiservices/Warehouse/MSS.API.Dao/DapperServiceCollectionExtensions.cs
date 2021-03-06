﻿using MSS.API.Model;
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
            services.AddTransient<IWarehouseRepo<Warehouse>, WarehouseRepo>();
            services.AddTransient<ISparePartsRepo<SpareParts>, SparePartsRepo>();
            services.AddTransient<IWarehouseAlarmRepo<WarehouseAlarm>, WarehouseAlarmRepo>();
            services.AddTransient<IStockOperationRepo<StockOperation>, StockOperationRepo>();
            services.AddTransient<IWarehouseAlarmHistoryRepo<WarehouseAlarmHistory>, WarehouseAlarmHistoryRepo>();
            services.AddTransient<IStorageLocationRepo<StorageLocation>, StorageLocationRepo>();
            // 配置列名映射
            FluentMapper.Initialize(config =>
            {
                //config.AddMap(new BaseEntityMap());
                config.AddMap(new WarehouseMap());
                config.AddMap(new SparePartsMap());
                config.AddMap(new WarehouseAlarmMap());
                config.AddMap(new StockOperationMap());
                config.AddMap(new StockOperationDetailMap());
                config.AddMap(new StockMap());
                config.AddMap(new StockDetailMap());
                config.AddMap(new StockSumMap());
                config.AddMap(new WarehouseAlarmHistoryMap());
                config.AddMap(new StorageLocationMap());
            });
            return services;
        }
    }
}
