using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class EarlyWarnningSetting : BaseEntity
    {
        public int EquipmentTypeID { get; set; }
        public string ParamID { get; set; }
        public string  ParamName { get; set; }
        public string ParamUnit { get; set; }
        public float ParamLimitUpper { get; set; }
        public float ParamLimitLower { get; set; }
        public bool IsActived { get; set; }
        public List<EarlyWarnningSettingEx> SettingEx { get; set; }

        public string EquipmentTypeName { get; set; }
        public string UserName { get; set;}
        /// <summary>
        /// 专家库ID
        /// </summary>
        public int? Expert { get; set; }
    }

    public class EarlyWarnningSettingMap : EntityMap<EarlyWarnningSetting>
    {
        public EarlyWarnningSettingMap()
        {
            Map(o => o.EquipmentTypeID).ToColumn("equipment_type_id");
            Map(o => o.ParamID).ToColumn("param_id");
            Map(o => o.ParamName).ToColumn("param_name");
            Map(o => o.ParamUnit).ToColumn("param_unit");
            Map(o => o.ParamLimitUpper).ToColumn("param_limit_upper");
            Map(o => o.ParamLimitLower).ToColumn("param_limit_lower");
            Map(o => o.IsActived).ToColumn("is_actived");
            Map(o => o.SettingEx).Ignore();

            Map(o => o.EquipmentTypeName).ToColumn("eqp_type_name");
            Map(o => o.UserName).ToColumn("user_name");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }
}
