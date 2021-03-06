﻿using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class Equipment : BaseEntity
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string TName { get; set; }
        public string AssetNo { get; set; }
        public string Model { get; set; }
        public int SubSystem { get; set; }
        public string SubSystemName { get; set; }
        public int Team { get; set; }
        public string TeamPath { get; set; }
        public int TopOrg { get; set; }
        public string TeamName { get; set; }
        public string BarCode { get; set; }
        public string Desc { get; set; }
        public int? Supplier { get; set; }
        public string SupplierName { get; set; }
        public int? Manufacturer { get; set; }
        public string ManufacturerName { get; set; }
        public string SerialNo { get; set; }
        public double? RatedVoltage { get; set; }
        public double? RatedCurrent { get; set; }
        public double? RatedPower { get; set; }
        public int Location { get; set; }
        /// <summary>
        /// 哪张表的位置ID
        /// </summary>
        public int LocationBy { get; set; }
        public string LocationPath { get; set; }
        public string LocationName { get; set; }
        public DateTime Online { get; set; }
        public int? Life { get; set; }
        public int MediumRepair { get; set; }
        public int LargeRepair { get; set; }
        public DateTime? OnlineAgain { get; set; }
        public string CreatedName { get; set; }
        public string UpdatedName { get; set; }
        public DateTime NextMediumRepairDate { get; set; }
        public DateTime NextLargeRepairDate { get; set; }
    }

    public class EquipmentMap : EntityMap<Equipment>
    {
        public EquipmentMap()
        {
            Map(o => o.Code).ToColumn("eqp_code");
            Map(o => o.Name).ToColumn("eqp_name");
            Map(o => o.Type).ToColumn("eqp_type");
            Map(o => o.TName).ToColumn("type_name");

            Map(o => o.AssetNo).ToColumn("eqp_asset_number");
            Map(o => o.Model).ToColumn("eqp_model");
            Map(o => o.SubSystem).ToColumn("sub_system");
            Map(o => o.SubSystemName).ToColumn("sub_code_name");
            Map(o => o.Team).ToColumn("team");
            Map(o => o.TeamPath).ToColumn("team_path");
            Map(o => o.TopOrg).ToColumn("top_org");
            Map(o => o.TeamName).ToColumn("name");
            Map(o => o.BarCode).ToColumn("bar_code");
            Map(o => o.Desc).ToColumn("discription");

            Map(o => o.Supplier).ToColumn("supplier");
            Map(o => o.SupplierName).ToColumn("sname");
            Map(o => o.Manufacturer).ToColumn("manufacturer");
            Map(o => o.ManufacturerName).ToColumn("mname");
            Map(o => o.SerialNo).ToColumn("serial_number");

            Map(o => o.RatedVoltage).ToColumn("rated_voltage");
            Map(o => o.RatedCurrent).ToColumn("rated_current");
            Map(o => o.RatedPower).ToColumn("rated_power");

            Map(o => o.Location).ToColumn("location");
            Map(o => o.LocationPath).ToColumn("location_path");
            Map(o => o.LocationBy).ToColumn("location_by");
            Map(o => o.LocationName).ToColumn("AreaName");
            Map(o => o.Online).ToColumn("online_date");
            Map(o => o.Life).ToColumn("life");

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

            Map(o => o.NextMediumRepairDate).ToColumn("next_medium_repair_date");
            Map(o => o.NextLargeRepairDate).ToColumn("next_large_repair_date");
        }
    }

    public class EquipmntView
    {
        public int Total { get; set; }
        public List<Equipment> Rows { get; set; }
    }
}
