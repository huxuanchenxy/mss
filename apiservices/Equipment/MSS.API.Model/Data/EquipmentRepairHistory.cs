using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class EquipmentRepairHistory : BaseEntity
    {
        public int Trouble { get; set; }
        public string TroubleCode { get; set; }
        public int Eqp { get; set; }
        public string EqpCode { get; set; }
        public string EqpName { get; set; }
        public string EqpPath { get; set; }
        public string Desc { get; set; }
        public string CreatedName { get; set; }
        public string UpdatedName { get; set; }
        public int? Type { get; set; }
        public string UploadFiles { get; set; }
    }
    public class EquipmentRepairHistoryMap : EntityMap<EquipmentRepairHistory>
    {
        public EquipmentRepairHistoryMap()
        {
            Map(o => o.TroubleCode).ToColumn("code");
            Map(o => o.EqpCode).ToColumn("eqp_code");
            Map(o => o.EqpName).ToColumn("eqp_name");
            Map(o => o.EqpPath).ToColumn("eqp_path");
            Map(o => o.Desc).ToColumn("description");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedName).ToColumn("created_name");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedName).ToColumn("updated_name");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
        }
    }

    public class EquipmentRepairHistoryQueryParm : BaseQueryParm
    {
        public int? Trouble { get; set; }
        public int? Eqp { get; set; }
        public string Desc { get; set; }
    }
    public class EquipmentRepairHistoryView
    {
        public List<EquipmentRepairHistory> rows { get; set; }
        public int total { get; set; }
    }
}
