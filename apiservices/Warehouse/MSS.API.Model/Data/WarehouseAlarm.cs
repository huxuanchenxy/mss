using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class WarehouseAlarm : BaseEntity
    {
        public int Warehouse { get; set; }
        public string WarehouseName { get; set; }
        public int SpareParts { get; set; }
        public string SparePartsName { get; set; }
        public int SafeStorage { get; set; }
        public string CreatedName { get; set; }
        public string UpdatedName { get; set; }
        public WarehouseAlarmHistory WAlarmHistory { get; set; }
        public int IsAlarm { get; set; }
        public bool IsStockUpdate { get; set; }
        public bool IsStockSumUpdate { get; set; }
        public int IsStockSumAlarm { get; set; }
    }

    public class WarehouseAlarmMap : EntityMap<WarehouseAlarm>
    {
        public WarehouseAlarmMap()
        {
            Map(o => o.Warehouse).ToColumn("warehouse");
            Map(o => o.WarehouseName).ToColumn("name");

            Map(o => o.SpareParts).ToColumn("spare_parts");
            Map(o => o.SparePartsName).ToColumn("spName");
            Map(o => o.SafeStorage).ToColumn("safe_storage");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedName).ToColumn("created_name");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedName).ToColumn("updated_name");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }

    public class WarehouseAlarmQueryParm : BaseQueryParm
    {
        public int? SearchType { get; set; }
        public int SearchSpareParts { get; set; }
    }
}
