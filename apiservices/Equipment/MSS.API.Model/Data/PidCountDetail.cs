
using Dapper.FluentMap.Mapping;
using System.Collections.Generic;

// Coded by admin 2020/2/19 9:29:56
namespace MSS.API.Model.Data
{
    public class PidCountDetailParm : BaseQueryParm
    {

    }
    public class PidCountDetailPageView
    {
        public List<PidCountDetail> rows { get; set; }
        public int total { get; set; }
    }

    public class PidCountDetail : BaseEntity
    {
        public int PidCountId { get; set; }
        public long CapacityCountOld { get; set; }
        public long CapacityCountNew { get; set; }
        public long RemainCountOld { get; set; }
        public long RemainCountNew { get; set; }
        public long UsedCountOld { get; set; }
        public long UsedCountNew { get; set; }
        public long RemindCountOld { get; set; }
        public long RemindCountNew { get; set; }
    }

    public class PidCountDetailMap : EntityMap<PidCountDetail>
    {
        public PidCountDetailMap()
        {
            Map(o => o.ID).ToColumn("id");
            Map(o => o.PidCountId).ToColumn("pid_count_id");
            Map(o => o.CapacityCountOld).ToColumn("capacity_count_old");
            Map(o => o.CapacityCountNew).ToColumn("capacity_count_new");
            Map(o => o.RemainCountOld).ToColumn("remain_count_old");
            Map(o => o.RemainCountNew).ToColumn("remain_count_new");
            Map(o => o.UsedCountOld).ToColumn("used_count_old");
            Map(o => o.UsedCountNew).ToColumn("used_count_new");
            Map(o => o.RemindCountOld).ToColumn("remind_count_old");
            Map(o => o.RemindCountNew).ToColumn("remind_count_new");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
        }
    }

}