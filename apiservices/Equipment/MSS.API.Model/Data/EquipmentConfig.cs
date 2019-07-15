using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class EquipmentConfig : BaseEntity
    {
        public EmReminder Reminder { get; set; }
        public int BeforeDead { get; set; }
        public int BeforeMaintain { get; set; }

        public string TextTemplate { get; set; }

        public bool Published { get; set; }

        public string CreatedName { get; set; }
        public string UpdatedName { get; set; }

    }

    public enum EmReminder
    {
        MobileText = 1,//短信提醒
        EmailText = 2,//邮件提醒
        MobileEmailText = 3 //同时短信和邮件提醒
    }

    public class EquipmentConfigMap : EntityMap<EquipmentConfig>
    {
        public EquipmentConfigMap()
        {
            Map(o => o.Reminder).ToColumn("reminder");
            Map(o => o.BeforeDead).ToColumn("before_dead");
            Map(o => o.BeforeMaintain).ToColumn("before_maintain");
            Map(o => o.TextTemplate).ToColumn("text_template");
            Map(o => o.Published).ToColumn("published");
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedName).ToColumn("created_name");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedName).ToColumn("updated_name");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }





}
