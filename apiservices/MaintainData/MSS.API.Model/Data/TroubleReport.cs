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
        public int? TroubleStatus { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }

    /// <summary>
    /// dto
    /// </summary>
    public class TroubleReport
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public DateTime HappeningTime { get; set; }
        public DateTime ReportedTime { get; set; }
        public int Line { get; set; }
        public string LineName { get; set; }
        public int StartLocation { get; set; }
        public int StartLocationBy { get; set; }
        public string StartLocationPath { get; set; }
        public string StartLocationName { get; set; }
        public int? EndLocation { get; set; }
        public int? EndLocationBy { get; set; }
        public string EndLocationName { get; set; }
        public string UrgentRepairOrder { get; set; }
        //public int MetroTrain { get; set; }
        //public int MetroCarriage { get; set; }
        public string Eqps { get; set; }
        public int? Level { get; set; }
        public string LevelName { get; set; }
        public int ReportedCompany { get; set; }
        public string ReportedCompanyPath { get; set; }
        public string ReportedCompanyName { get; set; }
        public int ReportedBy { get; set; }
        public string ReportedByName { get; set; }
        public string Mobile { get; set; }
        public string Desc { get; set; }
        //public int Type { get; set; }
        //public string TypeName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        //public string Solution { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime AcceptedTime { get; set; }
        public int UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
        public DateTime UpdatedTime { get; set; }
        public string UploadFiles { get; set; }
        //前端显示格式的json字符串
        public string RepairCompany { get; set; }
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
            Map(o => o.StartLocationPath).ToColumn("start_location_path");
            Map(o => o.EndLocation).ToColumn("end_location");
            Map(o => o.EndLocationName).ToColumn("AreaName");
            Map(o => o.EndLocationBy).ToColumn("end_location_by");
            Map(o => o.UrgentRepairOrder).ToColumn("urgent_repair_order");
            //Map(o => o.MetroTrain).ToColumn("metro_train");
            //Map(o => o.MetroCarriage).ToColumn("metro_carriage");
            Map(o => o.ReportedCompany).ToColumn("reported_company");
            Map(o => o.ReportedCompanyPath).ToColumn("reported_company_path");
            Map(o => o.ReportedCompanyName).ToColumn("rcname");
            Map(o => o.LevelName).ToColumn("lname");

            Map(o => o.Desc).ToColumn("description");
            Map(o => o.HappeningTime).ToColumn("happening_time");
            //Map(o => o.Type).ToColumn("type");
            //Map(o => o.TypeName).ToColumn("typeName");
            Map(o => o.Status).ToColumn("status");
            Map(o => o.StatusName).ToColumn("name");
            //Map(o => o.Solution).ToColumn("solution");
            Map(o => o.ReportedBy).ToColumn("reported_by");
            Map(o => o.ReportedByName).ToColumn("user_name");
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedByName).ToColumn("cname");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.AcceptedTime).ToColumn("accepted_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedByName).ToColumn("uname");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
        }
    }

    public class TroubleReportView
    {
        public List<TroubleReport> rows { get; set; }
        public int total { get; set; }
    }

    public class TroubleEqp
    {
        public int ID { get; set; }
        public int Trouble { get; set; }
        public int Org { get; set; }
        public string OrgName { get; set; }
        public int Eqp { get; set; }
        public string EqpName { get; set; }
    }

    public class TroubleEqpMap : EntityMap<TroubleEqp>
    {
        public TroubleEqpMap()
        {
            Map(o => o.OrgName).ToColumn("name");
            Map(o => o.EqpName).ToColumn("eqp_name");
        }
    }

}