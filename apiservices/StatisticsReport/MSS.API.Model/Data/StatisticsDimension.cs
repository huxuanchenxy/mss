using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class StatisticsDimension : BaseEntity
    {
        public int EqpID { get; set; }
        public string EqpName { get; set; }
        public int EqpTypeID { get; set; }
        public string EqpTypeName { get; set; }
        public int SupplierID { get; set; }
        public string SupplierName { get; set; }
        public int ManufacturerID { get; set; }
        public string ManufacturerName { get; set; }
        public int SubSystemID { get; set; }
        public string SubSystemName { get; set; }
        public int LocationLevel1 { get; set; }
        public string LocationLevel1Name { get; set; }
        public int LocationLevel2 { get; set; }
        public string LocationLevel2Name { get; set; }
        public int LocationLevel3 { get; set; }
        public string LocationLevel3Name { get; set; }
        public int TopOrgID { get; set; }
        public string TopOrgName { get; set; }
        public int TeamID { get; set; }
        public string TeamName { get; set; }
        public string TeamPath { get; set; }
        public string MajorID { get; set; }
        public string MajorName { get; set; }

    }

    public class StatisticsDimensionMap : EntityMap<StatisticsDimension>
    {
        public StatisticsDimensionMap()
        {
            Map(o => o.EqpID).ToColumn("eqp_id");
            Map(o => o.EqpName).ToColumn("eqp_name");
            Map(o => o.EqpTypeID).ToColumn("eqp_type_id");
            Map(o => o.EqpTypeName).ToColumn("eqp_type_name");
            Map(o => o.SupplierID).ToColumn("supplier_id");
            Map(o => o.SupplierName).ToColumn("supplier_name");
            Map(o => o.ManufacturerID).ToColumn("manufacturer_id");
            Map(o => o.ManufacturerName).ToColumn("manufacturer_name");
            Map(o => o.SubSystemID).ToColumn("sub_system_id");
            Map(o => o.SubSystemName).ToColumn("sub_system_name");
            Map(o => o.LocationLevel1).ToColumn("location_level1");
            Map(o => o.LocationLevel1Name).ToColumn("location_level1_name");
            Map(o => o.LocationLevel2).ToColumn("location_level2");
            Map(o => o.LocationLevel2Name).ToColumn("location_level2_name");
            Map(o => o.LocationLevel3).ToColumn("location_level3");
            Map(o => o.LocationLevel3Name).ToColumn("location_level3_name");
            Map(o => o.TopOrgID).ToColumn("top_org_id");
            Map(o => o.TopOrgName).ToColumn("top_org_name");
            Map(o => o.TeamID).ToColumn("team_id");
            Map(o => o.TeamName).ToColumn("team_name");
            Map(o => o.TeamPath).ToColumn("team_path");
            Map(o => o.MajorID).ToColumn("major_id");
            Map(o => o.MajorName).ToColumn("major_name");
        }
    }

}
