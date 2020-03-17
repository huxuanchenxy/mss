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
        public TroubleView? MenuView { get; set; }
        /// <summary>
        /// 我的接修和审核时用到
        /// </summary>
        public int RepairCompany { get; set; }
        /// <summary>
        /// 我的在处理故障用到，维修人员所属部门/班组
        /// </summary>
        public int OrgNode { get; set; }
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
        /// <summary>
        /// 转发专用，转发前的接修单位名称
        /// </summary>
        public string OldReportedCompanyName { get; set; }
        public string ReportedCompanyName { get; set; }
        public int ReportedBy { get; set; }
        public string ReportedByName { get; set; }
        public string Mobile { get; set; }
        public string Desc { get; set; }
        //public int Type { get; set; }
        //public string TypeName { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; }
        public int LastOperation { get; set; }
        public string LastOperationName { get; set; }
        //public string Solution { get; set; }
        public int ChartType { get; set; }
        public string ChartTypePath { get; set; }
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
        public List<TroubleDeal> TroubleDeals { get; set; }
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
            Map(o => o.LastOperation).ToColumn("last_operation");
            Map(o => o.LastOperationName).ToColumn("loname");
            Map(o => o.ChartType).ToColumn("chart_type");
            Map(o => o.ChartTypePath).ToColumn("chart_type_path");
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
        public int RepairCompany { get; set; }
    }

    public class TroubleEqp
    {
        public int ID { get; set; }
        public int Trouble { get; set; }
        public int Org { get; set; }
        public string OrgName { get; set; }
        public int Eqp { get; set; }
        public string EqpName { get; set; }
        public int? OrgNode { get; set; }
        public string OrgNodeName { get; set; }
        public string OrgPath { get; set; }
        public string OrgPathTmp { get; set; }
        public int? AssignedBy { get; set; }
        public string AssignedByName { get; set; }
        public DateTime? AssignedTime { get; set; }
        public int TroubleLevel { get; set; }
    }

    public class TroubleEqpMap : EntityMap<TroubleEqp>
    {
        public TroubleEqpMap()
        {
            Map(o => o.OrgName).ToColumn("name");
            Map(o => o.EqpName).ToColumn("eqp_name");
            Map(o => o.OrgNode).ToColumn("org_node");
            Map(o => o.OrgNodeName).ToColumn("onname");
            Map(o => o.AssignedBy).ToColumn("assigned_by");
            Map(o => o.AssignedByName).ToColumn("user_name");
            Map(o => o.AssignedTime).ToColumn("assigned_time");
            Map(o => o.OrgPath).ToColumn("org_path");
            Map(o => o.OrgPathTmp).ToColumn("team_path");
            Map(o => o.TroubleLevel).ToColumn("level");
        }
    }

    public class TroubleHistory
    {
        public int ID { get; set; }
        public int Trouble { get; set; }
        public string Code { get; set; }
        public int OrgTop { get; set; }
        public string OrgTopName { get; set; }
        public TroubleOperation Operation { get; set; }
        public string OperationName { get; set; }
        public string Content { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedTime { get; set; }
    }

    public class TroubleHistoryMap : EntityMap<TroubleHistory>
    {
        public TroubleHistoryMap()
        {
            Map(o => o.OrgTop).ToColumn("org_top");
            Map(o => o.OrgTopName).ToColumn("otname");
            Map(o => o.OperationName).ToColumn("name");
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedByName).ToColumn("user_name");
            Map(o => o.CreatedTime).ToColumn("created_time");
        }
    }

    public class TroubleDeal
    {
        public int ID { get; set; }
        public int Trouble { get; set; }
        public int OrgTop { get; set; }
        public string OrgTopName { get; set; }
        public string Code { get; set; }
        public int? DealBy { get; set; }
        public string DealByName { get; set; }
        public DateTime? ArrivedTime { get; set; }
        public DateTime? FinishedTime { get; set; }
        public string Process { get; set; }
        public string SparePartsReplace { get; set; }
        public string RepairEvaluation { get; set; }
        public string RepairReason { get; set; }
        public int IsSure { get; set; }
        public string UnpassReason { get; set; }
        public int? SureBy { get; set; }
        public string SureByName { get; set; }
        public DateTime? SureTime { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public DateTime CreatedTime { get; set; }
        public int UpdateBy { get; set; }
        public string UpdateByName { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    public class TroubleDealMap : EntityMap<TroubleDeal>
    {
        public TroubleDealMap()
        {
            Map(o => o.ArrivedTime).ToColumn("arrived_time");
            Map(o => o.FinishedTime).ToColumn("finished_time");
            Map(o => o.OrgTop).ToColumn("org_top");
            Map(o => o.OrgTopName).ToColumn("name");
            Map(o => o.DealBy).ToColumn("deal_by");
            Map(o => o.DealByName).ToColumn("dname");
            Map(o => o.SparePartsReplace).ToColumn("spareparts_replace");
            Map(o => o.RepairEvaluation).ToColumn("repair_evaluation");
            Map(o => o.RepairReason).ToColumn("repair_reason");
            Map(o => o.IsSure).ToColumn("is_sure");
            Map(o => o.UnpassReason).ToColumn("unpass_reason");
            Map(o => o.SureBy).ToColumn("sure_by");
            Map(o => o.SureByName).ToColumn("sname");
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedByName).ToColumn("user_name");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdateBy).ToColumn("update_by");
            Map(o => o.UpdateByName).ToColumn("uname");
            Map(o => o.UpdateTime).ToColumn("update_time");
        }
    }

}