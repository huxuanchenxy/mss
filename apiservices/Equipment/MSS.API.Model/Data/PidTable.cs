
using Dapper.FluentMap.Mapping;
using System.Collections.Generic;

// Coded by admin 2020/2/13 9:33:27
namespace MSS.API.Model.Data
{
    public class PidTableParm : BaseQueryParm
    {

    }
    public class PidTablePageView
    {
        public List<PidTable> rows { get; set; }
        public int total { get; set; }
    }

    public class PidTable : BaseEntity
    {
        public string PID { get; set; }
        public int EqpId { get; set; }
        public string Prop { get; set; }
        public string Des { get; set; }
        public int PidType { get; set; }
        public string UT { get; set; }
        public float UP { get; set; }
        public float DW { get; set; }
        public float UUP { get; set; }
        public float DDW { get; set; }
        public string EqpCode { get; set; }
        public string EqpName { get; set; }
        public string TypeName { get; set; }
    }

    public class PidTableMap : EntityMap<PidTable>
    {
        public PidTableMap()
        {
            Map(o => o.PID).ToColumn("PID");
            Map(o => o.EqpId).ToColumn("eqp_id");
            Map(o => o.Prop).ToColumn("prop");
            Map(o => o.Des).ToColumn("Des");
            Map(o => o.PidType).ToColumn("pid_type");
            Map(o => o.UT).ToColumn("UT");
            Map(o => o.UP).ToColumn("UP");
            Map(o => o.DW).ToColumn("DW");
            Map(o => o.UUP).ToColumn("UUP");
            Map(o => o.DDW).ToColumn("DDW");
            Map(o => o.EqpCode).ToColumn("eqp_code");
            Map(o => o.EqpName).ToColumn("eqp_name");
            Map(o => o.TypeName).ToColumn("type_name");
        }
    }

}