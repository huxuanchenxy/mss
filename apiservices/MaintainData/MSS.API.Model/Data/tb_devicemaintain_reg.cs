using Dapper.FluentMap.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.Data
{
    public class tb_devicemaintain_reg : BaseEntity
    {
        //public int id { get; set; }
        public int device_type_id { get; set; }
        public int device_id { get; set; }
        public int team_group_id { get; set; }
        public int director_id { get; set; }
        public DateTime maintain_date { get; set; }

        public string detail_desc { get; set; }
        //public int is_deleted { get; set; }
        //public DateTime create_time { get; set; }
        //public int create_by { get; set; }
        //public DateTime update_time { get; set; }
        //public int update_by { get; set; }
    }


    /// <summary>
    /// model
    /// </summary>
    public class tb_devicemaintain_regMap : EntityMap<tb_devicemaintain_reg>
    {
        public tb_devicemaintain_regMap()
        {
            Map(o => o.ID).ToColumn("id");
            Map(o => o.device_type_id).ToColumn("device_type_id");
            Map(o => o.device_id).ToColumn("device_id");
            Map(o => o.team_group_id).ToColumn("team_group_id");
            Map(o => o.director_id).ToColumn("director_id");
            Map(o => o.maintain_date).ToColumn("maintain_date"); 
            Map(o => o.detail_desc).ToColumn("detail_desc"); 
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.Is_deleted).ToColumn("Is_deleted");

        }
    }
}
