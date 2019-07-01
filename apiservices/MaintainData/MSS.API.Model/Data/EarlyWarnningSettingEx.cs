using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class EarlyWarnningSettingEx:BaseEntity
    {
        public int EarlyWarnningID { get; set; }
        public int ParamID { get; set; }
        public int  ParamLimitType { get; set; }
        public int ParamLimitValue { get; set; }
    }

    public class EarlyWarnningSettingExMap : EntityMap<EarlyWarnningSettingEx>
    {
        public EarlyWarnningSettingExMap()
        {
            Map(o => o.EarlyWarnningID).ToColumn("early_warnning_id");
            Map(o => o.ParamID).ToColumn("param_id");
            Map(o => o.ParamLimitType).ToColumn("param_limit_type");
            Map(o => o.ParamLimitValue).ToColumn("param_limit_value");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }
}
