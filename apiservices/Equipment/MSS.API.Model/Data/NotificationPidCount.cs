
using Dapper.FluentMap.Mapping;
using System.Collections.Generic;

// Coded by admin 2020/2/20 14:38:50
namespace MSS.API.Model.Data
{
    public class NotificationPidcountParm : BaseQueryParm
    {
        public int status { get; set; }
    }
    public class NotificationPidcountPageView
    {
        public List<NotificationPidcount> rows { get; set; }
        public int total { get; set; }
    }

    public class NotificationPidcount : BaseEntity
    {
        public int PidCountId { get; set; }
        public string PidCountName { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }
    }

    public class NotificationPidcountMap : EntityMap<NotificationPidcount>
    {
        public NotificationPidcountMap()
        {
            Map(o => o.ID).ToColumn("ID");
            Map(o => o.PidCountId).ToColumn("pid_count_id");
            Map(o => o.PidCountName).ToColumn("pid_count_name");
            Map(o => o.Content).ToColumn("content");
            Map(o => o.Status).ToColumn("status");
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }

}