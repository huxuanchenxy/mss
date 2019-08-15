using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class EarlyWarnning : BaseEntity
    {
        public string Pid { get; set; }
        
        public int  EqpID { get; set; }
        public string Content { get; set; }
        public double CurValue { get; set; }
        public int Status { get; set; }
        public string EqpCode { get; set; }
        public string EqpName { get; set; }
        public int EqpTypeID { get; set; }
        public string EqpTypeName { get; set; }
    }

    public class EarlyWarnningMap : EntityMap<EarlyWarnning>
    {
        public EarlyWarnningMap()
        {
            Map(o => o.Pid).ToColumn("pid");
            Map(o => o.EqpID).ToColumn("eqp_id");
            Map(o => o.Content).ToColumn("content");
            Map(o => o.CurValue).ToColumn("cur_value");
            Map(o => o.Status).ToColumn("status");
            Map(o => o.EqpCode).ToColumn("eqp_code");
            Map(o => o.EqpName).ToColumn("eqp_name");
            Map(o => o.EqpTypeID).ToColumn("eqp_type");
            Map(o => o.EqpTypeName).ToColumn("type_name");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }
}
