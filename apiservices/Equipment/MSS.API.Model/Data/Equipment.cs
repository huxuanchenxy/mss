using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class Equipment:BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string AssetNo { get; set; }
        public string Model { get; set; }
        public int SubSystem { get; set; }
        public int Team { get; set; }
        public string BarCode { get; set; }
        public string Desc { get; set; }
        public int Supplier { get; set; }
        public int Manufacturer { get; set; }
        public string SerialNo { get; set; }
        public double RatedVoltage { get; set; }
        public double RatedCurrent { get; set; }
        public double RatedPower { get; set; }
        public int Location { get; set; }
        /// <summary>
        /// 哪张表的位置ID
        /// </summary>
        public string LocationBy { get; set; }
        public DateTime Online { get; set; }
        public string Life { get; set; }
        public string PathPic { get; set; }
        public int MediumRepair { get; set; }
        public int LargeRepair { get; set; }
        public DateTime OnlineAgain { get; set; }
        public string CreatedName { get; set; }
        public string UpdatedName { get; set; }
    }

    public class EquipmentMap : EntityMap<Equipment>
    {
        public EquipmentMap()
        {
            Map(o => o.Code).ToColumn("eqp_code");
            Map(o => o.Name).ToColumn("eqp_name");
            Map(o => o.Type).ToColumn("eqp_type");

            Map(o => o.AssetNo).ToColumn("eqp_asset_number");
            Map(o => o.Model).ToColumn("eqp_model");
            Map(o => o.SubSystem).ToColumn("sub_system");
            Map(o => o.Team).ToColumn("team");
            Map(o => o.BarCode).ToColumn("bar_code");
            Map(o => o.Desc).ToColumn("discription");

            Map(o => o.Supplier).ToColumn("supplier");
            Map(o => o.Manufacturer).ToColumn("manufacturer");
            Map(o => o.SerialNo).ToColumn("serial_number");

            Map(o => o.RatedVoltage).ToColumn("rated_voltage");
            Map(o => o.RatedCurrent).ToColumn("rated_current");
            Map(o => o.RatedPower).ToColumn("rated_power");

            Map(o => o.Location).ToColumn("location");
            Map(o => o.LocationBy).ToColumn("location_by");
            Map(o => o.Online).ToColumn("online_date");
            Map(o => o.Life).ToColumn("life");

            Map(o => o.PathPic).ToColumn("eqp_pic");
            Map(o => o.MediumRepair).ToColumn("medium_repair");
            Map(o => o.LargeRepair).ToColumn("large_repair");
            Map(o => o.OnlineAgain).ToColumn("online_again");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedName).ToColumn("created_name");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedName).ToColumn("updated_name");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }

    public class EqpQueryParm:BaseQueryParm
    {
        public string SearchName { get; set; }
        public string SearchDesc { get; set; }
    }
}
