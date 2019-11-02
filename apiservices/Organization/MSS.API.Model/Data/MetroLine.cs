using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class MetroLine :BaseEntity
    {
        public string Code { get; set; }
        public string LineName { get; set; }
        public string Description { get; set; }

        public string UserName { get; set; }
    }

    public class MetroLineMap : EntityMap<MetroLine>
    {
        public MetroLineMap()
        {
            Map(o => o.LineName).ToColumn("line_name");
            Map(o => o.Description).ToColumn("description");
            Map(o => o.UserName).ToColumn("user_name");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }
}
