using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class Warehouse:BaseEntity
    {
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Contact { get; set; }
        public string Address { get; set; }
        public string CreatedName { get; set; }
        public string UpdatedName { get; set; }
    }

    public class WarehouseMap : EntityMap<Warehouse>
    {
        public WarehouseMap()
        {
            Map(o => o.Name).ToColumn("name");
            Map(o => o.Mobile).ToColumn("mobile");

            Map(o => o.Contact).ToColumn("contact");
            Map(o => o.Address).ToColumn("address");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedName).ToColumn("created_name");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedName).ToColumn("updated_name");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }

    public class WarehouseQueryParm:BaseQueryParm
    {
        public string SearchName { get; set; }
    }
}
