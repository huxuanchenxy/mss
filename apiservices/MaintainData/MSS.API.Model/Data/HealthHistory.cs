using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class HealthHistoryQueryParm : BaseQueryParm
    {
        public int? Eqp { get; set; }
        public int? Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    /// <summary>
    /// dto
    /// </summary>
    public class HealthHistory
    {
        public int ID { get; set; }
        public int Eqp { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public int EqpType { get; set; }
        public string EqpTypeName { get; set; }
        public int? TroubleLevel { get; set; }
        public string TroubleLevelName { get; set; }
        public double Val { get; set; }
        public int CorrelationID { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedTime { get; set; }
        public int CreatedYear { get; set; }
        public int CreatedMonth { get; set; }
        public int CreatedDay { get; set; }

    }

    /// <summary>
    /// model
    /// </summary>
    public class HealthHistoryMap : EntityMap<HealthHistory>
    {
        public HealthHistoryMap()
        {
            Map(o => o.ID).ToColumn("id");
            Map(o => o.TypeName).ToColumn("name");
            Map(o => o.EqpType).ToColumn("eqp_type");
            Map(o => o.EqpTypeName).ToColumn("type_name");
            Map(o => o.TroubleLevel).ToColumn("trouble_level");
            Map(o => o.TroubleLevelName).ToColumn("name");
            Map(o => o.CorrelationID).ToColumn("correlation_id"); 
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedByName).ToColumn("cname");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.CreatedYear).ToColumn("created_year");
            Map(o => o.CreatedMonth).ToColumn("created_month");
            Map(o => o.CreatedDay).ToColumn("created_day");
        }
    }

    public class HealthHistoryView
    {
        public List<HealthHistory> rows { get; set; }
        public int total { get; set; }
    }
}