using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class StorageLocation:BaseEntity
    {
        public string Name { get; set; }
        public int Warehouse { get; set; }
        public string WarehouseName { get; set; }
        public string Row { get; set; }
        public string Column { get; set; }
        public string Des { get; set; }
        public string CreatedName { get; set; }
        public string UpdatedName { get; set; }
    }

    public class StorageLocationMap : EntityMap<StorageLocation>
    {
        public StorageLocationMap()
        {
            Map(o => o.Name).ToColumn("name");
            Map(o => o.WarehouseName).ToColumn("wname");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedName).ToColumn("created_name");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedName).ToColumn("updated_name");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }

    public class StorageLocationQueryParm : BaseQueryParm
    {
        public string Name { get; set; }
        public int? Warehouse { get; set; }
    }
}
