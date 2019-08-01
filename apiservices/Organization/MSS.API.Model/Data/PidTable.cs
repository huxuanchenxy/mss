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
        public int EqpTypeID { get; set; }
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
            Map(o => o.EqpTypeID).ToColumn("eqp_type");
        }
    }
}
