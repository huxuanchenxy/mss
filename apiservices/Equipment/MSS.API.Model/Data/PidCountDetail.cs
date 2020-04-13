
using Dapper.FluentMap.Mapping;
using System.Collections.Generic;

// Coded by admin 2020/3/10 14:52:59
namespace MSS.API.Model.Data
{
    public class PidCountDetailParm : BaseQueryParm
    {
        public int PidCountId { get; set; }
    }
    public class PidCountDetailPageView
    {
        public List<PidCountDetail> rows { get; set; }
        public int total { get; set; }
    }

    public class PidCountDetail : BaseEntity
    {
        public int Id { get; set; }
        public int PidCountId { get; set; }
        public long CountOld { get; set; }
        public long CountNew { get; set; }
        public int DetailType { get; set; }
        public string DetailContent { get; set; }

        public string UpdatedName { get; set; }
        public string CreatedName { get; set; }
        public long ChangeCount { get; set; }
    }

    public class PidCountDetailMap : EntityMap<PidCountDetail>
    {
        public PidCountDetailMap()
        {
            Map(o => o.Id).ToColumn("id");
            Map(o => o.PidCountId).ToColumn("pid_count_id");
            Map(o => o.CountOld).ToColumn("count_old");
            Map(o => o.CountNew).ToColumn("count_new");
            Map(o => o.DetailType).ToColumn("detail_type");
            Map(o => o.DetailContent).ToColumn("detail_content");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedName).ToColumn("updated_name");
            Map(o => o.CreatedName).ToColumn("created_name");
        }
    }

}