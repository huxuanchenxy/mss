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
    public class WorkingApplication
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public int AppPart { get; set; }
        public string AppPartName { get; set; }
        public string ApplicantName { get; set; }
        public int Applicant { get; set; }
        public string ApplicantMobile { get; set; }
        public int WorkingPart { get; set; }
        public string WorkingPartName { get; set; }
        public string ManagerName { get; set; }
        public string ManagerMobile { get; set; }
        public string WorkingLocation { get; set; }
        public int RegStation { get; set; }
        public string RegStationName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int EqpID { get; set; }
        public string EqpCode { get; set; }
        public string EqpName { get; set; }
        public string Content { get; set; }
        public string Detail { get; set; }
        public string CoordinationRequirements { get; set; }
        public string CoordinationSuggestions { get; set; }
        public string InfluencingSection { get; set; }
        public string InfluencingDescription { get; set; }
        public string SecurityMeasures { get; set; }
        public string ResponseSuggestions { get; set; }
        public int ResponseBy { get; set; }
        public string ResponseByName { get; set; }
        public DateTime ResponseTime { get; set; }
    }

    /// <summary>
    /// model
    /// </summary>
    public class WorkingApplicationMap : EntityMap<WorkingApplication>
    {
        public WorkingApplicationMap()
        {
            Map(o => o.ID).ToColumn("id");
            Map(o => o.EqpID).ToColumn("eqp");
            Map(o => o.EqpCode).ToColumn("eqp_code");
            Map(o => o.EqpName).ToColumn("eqp_name");
            Map(o => o.Type).ToColumn("type");
            Map(o => o.AppPart).ToColumn("application_part");
            Map(o => o.AppPartName).ToColumn("name");
            Map(o => o.Applicant).ToColumn("applicant");
            Map(o => o.ApplicantName).ToColumn("user_name");
            Map(o => o.ApplicantMobile).ToColumn("nobile");
            Map(o => o.WorkingPart).ToColumn("working_part");
            Map(o => o.WorkingPartName).ToColumn("wpname");
            Map(o => o.WorkingLocation).ToColumn("working_location");
            Map(o => o.RegStation).ToColumn("registration_station");
            Map(o => o.RegStationName).ToColumn("AreaName");
            Map(o => o.StartTime).ToColumn("start_time");
            Map(o => o.EndTime).ToColumn("end_time");
            Map(o => o.Content).ToColumn("content");
            Map(o => o.Detail).ToColumn("detail");
            Map(o => o.CoordinationRequirements).ToColumn("coordination_requirements");
            Map(o => o.CoordinationSuggestions).ToColumn("coordination_suggestions");
            Map(o => o.InfluencingSection).ToColumn("influencing_section");
            Map(o => o.InfluencingDescription).ToColumn("influencing_description");
            Map(o => o.SecurityMeasures).ToColumn("security_measures");
            Map(o => o.ResponseSuggestions).ToColumn("response_suggestions");
            Map(o => o.ResponseBy).ToColumn("response_by");
            Map(o => o.ResponseByName).ToColumn("rname");
            Map(o => o.ResponseTime).ToColumn("response_time");
        }
    }

    public class WorkingApplicationManager
    {
        public int ID { get; set; }
        public int WorkingApplication { get; set; }
        public int Manager { get; set; }
        public string ManagerName { get; set; }
        public string Mobile { get; set; }
    }

    public class WorkingApplicationManagerMap : EntityMap<WorkingApplicationManager>
    {
        public WorkingApplicationManagerMap()
        {
            Map(o => o.ID).ToColumn("id");
            Map(o => o.WorkingApplication).ToColumn("working_application");
            Map(o => o.Manager).ToColumn("manager");
            Map(o => o.ManagerName).ToColumn("user_name");
            Map(o => o.Mobile).ToColumn("mobile");
        }
    }

}