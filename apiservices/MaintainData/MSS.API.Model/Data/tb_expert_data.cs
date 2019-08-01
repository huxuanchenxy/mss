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
    public class tb_expert_data : BaseEntity
    {
       // public int Id { get; set; }
        public int device_type { get; set; }
        public string deviceTypeName { get; set; }

        public int deptid { get; set; }
        public string dept_path { get; set; }
        public string deptname { get; set; }

        public string keyword { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        public string video_file { get; set; }
        public string attch_file { get; set; }
        public string remark { get;set;}

        public string updated_name { get; set; }
        public string origin_file { get; set; }

        public string UploadFiles { get; set; }
    }

    /// <summary>
    /// model
    /// </summary>
    public class tb_expert_dataMap : EntityMap<tb_expert_data>
    {
        public tb_expert_dataMap()
        {
            Map(o => o.ID).ToColumn("id");
            Map(o => o.device_type).ToColumn("device_type");
            Map(o => o.deptid).ToColumn("deptid");
            Map(o => o.dept_path).ToColumn("dept_path");
            Map(o => o.keyword).ToColumn("keyword");
            Map(o => o.title).ToColumn("title");
            Map(o => o.content).ToColumn("content");
            Map(o => o.video_file).ToColumn("video_file");
            Map(o => o.attch_file).ToColumn("attch_file");
            Map(o => o.origin_file).ToColumn("origin_file");
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.Is_deleted).ToColumn("Is_deleted");

        }
    }
}