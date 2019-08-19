using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class StatisticsAlarm : BaseEntity
    {
        public int EqpID { get; set; }
        public int Level { get; set; }
        public DateTime? OccurTime { get; set; }
        public DateTime? RecoverTime { get; set; }
        public int ElapsedTime { get; set; }
        public string Prop { get; set; }
        public string Content { get; set; }
    }

    public class StatisticsAlarmMap : EntityMap<StatisticsAlarm>
    {
        public StatisticsAlarmMap()
        {
            Map(o => o.EqpID).ToColumn("eqp_id");
            Map(o => o.Level).ToColumn("level");
            Map(o => o.OccurTime).ToColumn("occur_time");
            Map(o => o.RecoverTime).ToColumn("recover_time");
            Map(o => o.ElapsedTime).ToColumn("elapsed_time");
            Map(o => o.Prop).ToColumn("prop");
            Map(o => o.Content).ToColumn("content");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }
}
