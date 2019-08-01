using Dapper.FluentMap.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.Data
{
    public class UploadFile
    {
        public int ID { get; set; }
        public string file_name { get; set; }
        public string file_path { get; set; }
        public int type { get; set; }
        public string TypeName { get; set; }
        public int SystemResource { get; set; }
        public string SystemResourceName { get; set; }
        public int entity_id { get; set; }

    }

    public class UploadFileMap : EntityMap<UploadFile>
    {
        public UploadFileMap()
        {
            Map(o => o.file_name).ToColumn("file_name");
            Map(o => o.file_path).ToColumn("file_path");
            Map(o => o.SystemResource).ToColumn("system_resource");
            Map(o => o.SystemResourceName).ToColumn("resourceName");
            Map(o => o.type).ToColumn("type");
            Map(o => o.TypeName).ToColumn("name");
            Map(o => o.entity_id).ToColumn("entity_id");
        }
    }
}
