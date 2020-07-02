using System;
using System.Collections.Generic;
using System.Text;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class PidTable
    {
        public string pid { get; set; }
        public int? EqpID { get; set;}
        public string prop { get; set; }
        public string Des { get; set; }
        public string UT { get; set; }
        public double? UP { get; set; }
        public double? DW { get; set; }
        public double? UUP { get; set; }
        public double? DDW { get; set; }
        public int PidType { get; set; }
        
        public string DeviceCode { get; set; }//rdb对应的设备编号
        public string DeviceType { get; set; }//rdb对应的设备类型
        public string StationId { get; set; }//rdb对应的站台编号
        public string ExpertId { get; set; }//rdb对应的专业编号
        // ex
        public string EqpCode { get; set; }
        public string EqpName { get; set; }
        public int EqpTypeID { get; set; }
        public string EqpTypeName { get; set; }
        public int TopOrg { get; set; }
    }

    public class PidTableMap : EntityMap<PidTable>
    {
        public PidTableMap()
        {
            Map(o => o.pid).ToColumn("PID");
            Map(o => o.EqpID).ToColumn("eqp_id");
            Map(o => o.prop).ToColumn("prop");
            Map(o => o.Des).ToColumn("Des");
            Map(o => o.UT).ToColumn("UT");
            Map(o => o.UP).ToColumn("UP");
            Map(o => o.DW).ToColumn("DW");
            Map(o => o.UUP).ToColumn("UUP");
            Map(o => o.DDW).ToColumn("DDW");
            Map(o => o.PidType).ToColumn("pid_type");

            // ex
            Map(o => o.EqpCode).ToColumn("eqp_code");
            Map(o => o.EqpName).ToColumn("eqp_name");
            Map(o => o.EqpTypeID).ToColumn("eqp_type");
            Map(o => o.EqpTypeName).ToColumn("type_name");
            Map(o => o.TopOrg).ToColumn("top_org");

            Map(o => o.DeviceCode).ToColumn("device_code");
            Map(o => o.DeviceType).ToColumn("device_type");
            Map(o => o.StationId).ToColumn("station_id");
            Map(o => o.ExpertId).ToColumn("expert_id");
        }
    }
}
