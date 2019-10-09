using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class WarehouseAlarmHistory
    {
        public int ID { get; set; }
        public int Warehouse { get; set; }
        public string WarehouseName { get; set; }
        public int SpareParts { get; set; }
        public string SparePartsName { get; set; }
        public int StockNo { get; set; }
        public int SafeStorage { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    public class WarehouseAlarmHistoryMap : EntityMap<WarehouseAlarmHistory>
    {
        public WarehouseAlarmHistoryMap()
        {
            Map(o => o.Warehouse).ToColumn("warehouse");
            Map(o => o.WarehouseName).ToColumn("name");

            Map(o => o.SpareParts).ToColumn("spare_parts");
            Map(o => o.SparePartsName).ToColumn("spName");
            Map(o => o.StockNo).ToColumn("stock_no");
            Map(o => o.SafeStorage).ToColumn("safe_storage");

            Map(o => o.CreatedTime).ToColumn("created_time");
        }
    }

    public class WarehouseAlarmHistoryQueryParm : BaseQueryParm
    {
        public int? SearchType { get; set; }
        public int? SearchSpareParts { get; set; }
        public DateTime? SearchStart { get; set; }
        public DateTime? SearchEnd { get; set; }
    }
}
