
using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
// Coded by admin 2020/4/28 10:40:21
namespace MSS.API.Model.Data
{
    public class CostChartParm
    {
        public int startYear { get; set; }
        public int endYear { get; set; }
    }
    public class CostChartPageView
    {
        public List<CostChart> rows { get; set; }
        public int total { get; set; }
    }

    public class CostChart : BaseEntity
    {
        public int Id { get; set; }
        public string CostName { get; set; }
        public int CostType { get; set; }
        public int Value { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
    }

    public class CostChartMap : EntityMap<CostChart>
    {
        public CostChartMap()
        {
            Map(o => o.Id).ToColumn("id");
            Map(o => o.CostName).ToColumn("cost_name");
            Map(o => o.CostType).ToColumn("cost_type");
            Map(o => o.Value).ToColumn("value");
            Map(o => o.Year).ToColumn("year");
            Map(o => o.Month).ToColumn("month");
            Map(o => o.Day).ToColumn("day");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }

}