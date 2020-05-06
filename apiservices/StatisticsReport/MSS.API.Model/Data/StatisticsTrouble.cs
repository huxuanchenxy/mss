using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class StatisticsTrouble : BaseEntity
    {
        public int EqpID { get; set; }
        public DateTime? OccurTime { get; set; }
        public DateTime? RecoverTime { get; set; }
        public int ElapsedTime { get; set; }
        public int TroubleID { get; set; }
        public int? TroubleType { get; set; }

        public string date { get; set; }
        public string num { get; set; }
        public string avgtime { get; set; }
        public string TroubleName { get; set; }

        public StatisticsDimension dimension { get; set; }
    }

    public class StatisticsTroubleRank
    {
        public int troublecount { get; set; }
        public string name { get; set; }
    }

    public class StatisticsResult
    {
        public int count { get; set; }
        public string name { get; set; }
    }

    public class StatisticsTroubleMap : EntityMap<StatisticsTrouble>
    {
        public StatisticsTroubleMap()
        {
            Map(o => o.EqpID).ToColumn("eqp_id");
            Map(o => o.OccurTime).ToColumn("occur_time");
            Map(o => o.RecoverTime).ToColumn("recover_time");
            Map(o => o.ElapsedTime).ToColumn("elapsed_time");
            Map(o => o.TroubleID).ToColumn("trouble_id");
            Map(o => o.TroubleType).ToColumn("trouble_type");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }
}
