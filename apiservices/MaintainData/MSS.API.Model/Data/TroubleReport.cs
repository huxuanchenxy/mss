using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    /// <summary>
    /// dto
    /// </summary>
    public class TroubleReport
    {
        public int ID { get; set; }
        public string Desc { get; set; }
        public int EqpID { get; set; }
        public string EqpCode { get; set; }
        public string EqpName { get; set; }
        public DateTime HappeningTime { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string Solution { get; set; }
        public int ReportedBy { get; set; }
        public string ReportedByName { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedTime { get; set; }
        public int AcceptedBy { get; set; }
        public string AcceptedByName { get; set; }
        public DateTime AcceptedTime { get; set; }
    }

    /// <summary>
    /// model
    /// </summary>
    public class TroubleReportMap : EntityMap<TroubleReport>
    {
        public TroubleReportMap()
        {
            Map(o => o.ID).ToColumn("id");
            Map(o => o.EqpID).ToColumn("eqp");
            Map(o => o.EqpCode).ToColumn("eqp_code");
            Map(o => o.Desc).ToColumn("description");
            Map(o => o.EqpName).ToColumn("eqp_name");
            Map(o => o.HappeningTime).ToColumn("happening_time");
            Map(o => o.Type).ToColumn("type");
            Map(o => o.TypeName).ToColumn("typeName");
            Map(o => o.Status).ToColumn("status");
            Map(o => o.StatusName).ToColumn("name");
            Map(o => o.Solution).ToColumn("solution");
            Map(o => o.ReportedBy).ToColumn("reported_by");
            Map(o => o.ReportedByName).ToColumn("user_name");
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedByName).ToColumn("cname");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.AcceptedBy).ToColumn("accepted_by");
            Map(o => o.AcceptedByName).ToColumn("aname");
            Map(o => o.AcceptedTime).ToColumn("accepted_time");
        }
    }
}