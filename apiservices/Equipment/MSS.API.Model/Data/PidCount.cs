
using Dapper.FluentMap.Mapping;
using System.Collections.Generic;

// Coded by admin 2020/2/17 14:17:39
namespace MSS.API.Model.Data
{
    public class PidCountParm : BaseQueryParm
    {
        public string nodeId { get; set; }
        public string nodeName { get; set; }
        public string nodeTip { get; set; }
    }
    public class PidCountPageView
    {
        public List<PidCount> rows { get; set; }
        public int total { get; set; }
    }

    public class PidCount : BaseEntity
    {
        public int Id { get; set; }
        public string NodeId { get; set; }
        public string NodeName { get; set; }
        public string NodeTip { get; set; }
        public long CapacityCount { get; set; }
        public long RemainCount { get; set; }
        public long UsedCount { get; set; }
        public long RemindCount { get; set; }
        public System.DateTime CreatedTime { get; set; }
        public int CreatedBy { get; set; }
        public System.DateTime UpdatedTime { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedName { get; set; }
        public string CreatedName { get; set; }
    }

    public class PidCountMap : EntityMap<PidCount>
    {
        public PidCountMap()
        {
            Map(o => o.Id).ToColumn("id");
            Map(o => o.NodeId).ToColumn("node_id");
            Map(o => o.NodeName).ToColumn("node_name");
            Map(o => o.NodeTip).ToColumn("node_tip");
            Map(o => o.CapacityCount).ToColumn("capacity_count");
            Map(o => o.RemainCount).ToColumn("remain_count");
            Map(o => o.UsedCount).ToColumn("used_count");
            Map(o => o.RemindCount).ToColumn("remind_count");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedName).ToColumn("updated_name");
            Map(o => o.CreatedName).ToColumn("created_name");
        }
    }

}