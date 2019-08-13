using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
using MSS.API.Model.DTO;

namespace MSS.API.Model.Data
{
    /// <summary>
    /// dto
    /// </summary>
    public class EmergencyPlan:BaseEntity
    {
        public string Scene { get; set; }
        public int? Dept { get; set; }
        public string DeptName { get; set; }
        public string DeptPath { get; set; }
        public string Keyword { get; set; }
        public string UploadFiles { get; set; }
        public string Type { get; set; }
        public string CreatedName { get; set; }
        public string UpdatedName { get; set; }
    }

    /// <summary>
    /// model
    /// </summary>
    public class EmergencyPlanMap : EntityMap<EmergencyPlan>
    {
        public EmergencyPlanMap()
        {
            Map(o => o.ID).ToColumn("id");
            Map(o => o.Scene).ToColumn("emergency_scene");
            Map(o => o.Dept).ToColumn("dept_id");
            Map(o => o.DeptName).ToColumn("name");
            Map(o => o.DeptPath).ToColumn("dept_path");
            Map(o => o.Keyword).ToColumn("keyword");
            Map(o => o.Type).ToColumn("type");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedName).ToColumn("created_name");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedName).ToColumn("updated_name");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.Is_deleted).ToColumn("is_del");
        }
    }

    public class EPlanQueryParm : BasePageParm
    {
        public string SearchName { get; set; }
        public string SearchDesc { get; set; }
        public int Type { get; set; }
    }

    public class EPlanView
    {
        public List<EmergencyPlan> rows { get; set; }
        public int total { get; set; }
    }

}