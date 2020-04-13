
using Dapper.FluentMap.Mapping;
using System.Collections.Generic;

// Coded by admin 2020/4/7 14:52:22
namespace MSS.API.Model.Data
{
    public class HealthChartSubsystemParm : BaseQueryParm
    {

    }
    public class HealthChartSubsystemPageView
    {
        public List<HealthChartSubsystem> rows { get; set; }
        public int total { get; set; }
    }

    public class HealthChartSubsystem : BaseEntity
    {
        public int Id { get; set; }
        public int SubSystemId { get; set; }
        public double ValAvg { get; set; }
        public List<double> ValArr { get; set; }
        public double ValMax { get; set; }
        public double ValMin { get; set; }
        public double ValMiddle { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }

    public class HealthChartSubsystemMap : EntityMap<HealthChartSubsystem>
    {
        public HealthChartSubsystemMap()
        {
            Map(o => o.Id).ToColumn("id");
            Map(o => o.SubSystemId).ToColumn("sub_system_id");
            Map(o => o.ValAvg).ToColumn("val_avg");
            Map(o => o.ValMax).ToColumn("val_max");
            Map(o => o.ValMin).ToColumn("val_min");
            Map(o => o.ValMiddle).ToColumn("val_middle");
            Map(o => o.Year).ToColumn("year");
            Map(o => o.Month).ToColumn("month");
            Map(o => o.Day).ToColumn("day");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.CreatedBy).ToColumn("created_by");
        }
    }

}