using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class SpareParts:BaseEntity
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public string Model { get; set; }
        public string Unit { get; set; }
        public int? EqpType { get; set; }
        public string EqpTypeName { get; set; }
        public int PlanPrice { get; set; }
        public string EnglishDes { get; set; }
        public string Remark { get; set; }
        public string CreatedName { get; set; }
        public string UpdatedName { get; set; }
    }

    public class SparePartsMap : EntityMap<SpareParts>
    {
        public SparePartsMap()
        {
            Map(o => o.Name).ToColumn("name");
            Map(o => o.Type).ToColumn("type");
            Map(o => o.TypeName).ToColumn("tname");

            Map(o => o.Model).ToColumn("model");
            Map(o => o.Unit).ToColumn("unit");
            Map(o => o.EqpType).ToColumn("eqp_type");
            Map(o => o.EqpTypeName).ToColumn("type_name");
            Map(o => o.PlanPrice).ToColumn("plan_price");
            Map(o => o.EnglishDes).ToColumn("english_des");
            Map(o => o.Remark).ToColumn("remark");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedName).ToColumn("created_name");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedName).ToColumn("updated_name");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }

    public class SparePartsQueryParm:BaseQueryParm
    {
        public string SearchName { get; set; }
        public int? SearchType { get; set; }
        public int? SearchEqpType { get; set; }
    }
}
