using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class EarlyWarnningExType:BaseEntity
    {
        public string PID { get; set; }
        public string PidName { get; set; }
        public string ParamUnit { get; set; }
    }

    public class EarlyWarnningExTypeMap : EntityMap<EarlyWarnningExType>
    {
        public EarlyWarnningExTypeMap()
        {
            Map(o => o.PID).ToColumn("pid");
            Map(o => o.PidName).ToColumn("pid_name");
            Map(o => o.ParamUnit).ToColumn("param_unit");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }
}
