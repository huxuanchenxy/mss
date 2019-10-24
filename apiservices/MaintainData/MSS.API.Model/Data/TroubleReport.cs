using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class TroubleReportParm : BaseQueryParm
    {
        public string TroubleReportDesc { get; set; }
        public string RepairDesc { get; set; }
    }

    /// <summary>
    /// dto
    /// </summary>
    public class TroubleReport
    {
        public int ID { get; set; }
        public DateTime HappeningTime { get; set; }
        public DateTime ReportedTime { get; set; }
        public int Line { get; set; }
        public string LineName { get; set; }
        public int StartLocation { get; set; }
        public int StartLocationBy { get; set; }
        public string StartLocationName { get; set; }
        public int? EndLocation { get; set; }
        public int? EndLocationBy { get; set; }
        public string EndLocationName { get; set; }
        public string UrgentRepairOrder { get; set; }
        public int MetroTrain { get; set; }
        public int MetroCarriage { get; set; }
        public int AcceptedCompany { get; set; }
        public string AcceptedCompanyName { get; set; }
        public int? EqpID { get; set; }
        public string EqpCode { get; set; }
        public string EqpName { get; set; }
        public int? Level { get; set; }
        public string LevelName { get; set; }
        public int ReportedCompany { get; set; }
        public string ReportedCompanyName { get; set; }
        public int ReportedBy { get; set; }
        public string ReportedByName { get; set; }
        public string Mobile { get; set; }
        public string Desc { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public string Solution { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedTime { get; set; }
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
            Map(o => o.LineName).ToColumn("line_name");
            Map(o => o.ReportedTime).ToColumn("reported_time");
            Map(o => o.StartLocation).ToColumn("start_location");
            Map(o => o.StartLocationBy).ToColumn("start_location_by");
            Map(o => o.EndLocation).ToColumn("end_location");
            Map(o => o.EndLocationBy).ToColumn("end_location_by");
            Map(o => o.UrgentRepairOrder).ToColumn("urgent_repair_order");
            Map(o => o.MetroTrain).ToColumn("metro_train");
            Map(o => o.MetroCarriage).ToColumn("metro_carriage");
            Map(o => o.ReportedCompany).ToColumn("reported_company");
            Map(o => o.ReportedCompanyName).ToColumn("rcname");
            Map(o => o.ReportedBy).ToColumn("reported_by");
            Map(o => o.ReportedByName).ToColumn("rname");
            Map(o => o.LevelName).ToColumn("lname");

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
            Map(o => o.AcceptedCompany).ToColumn("accepted_company");
            Map(o => o.AcceptedCompanyName).ToColumn("acname");
            Map(o => o.AcceptedTime).ToColumn("accepted_time");
        }
    }

    public class TroubleReportView
    {
        public List<TroubleReport> rows { get; set; }
        public int total { get; set; }
    }
}