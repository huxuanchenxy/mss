using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class Equipment:BaseEntity
    {
        public string EqpCode { get; set; }
        public string EqpName { get; set; }
        public int EqpType { get; set; }
        public int TopOrg { get; set;}
        public DateTime? OnlineDate { get; set; }
        public DateTime? OnlineAgain { get; set; }
        public int LifeCycle { get; set; }
        public int MediumRepair { get; set; }
        public int LargeRepair { get; set; }

    }

    public class EquipmentMap : EntityMap<Equipment>
    {
        public EquipmentMap()
        {
            Map(o => o.EqpCode).ToColumn("eqp_code");
            Map(o => o.EqpName).ToColumn("eqp_name");
            Map(o => o.EqpType).ToColumn("eqp_type");
            Map(o => o.TopOrg).ToColumn("top_org");
            Map(o => o.OnlineDate).ToColumn("online_date");
            Map(o => o.OnlineAgain).ToColumn("online_again");
            Map(o => o.LifeCycle).ToColumn("life");
            Map(o => o.MediumRepair).ToColumn("medium_repair");
            Map(o => o.LargeRepair).ToColumn("large_repair");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }
}
