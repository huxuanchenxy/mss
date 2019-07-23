using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
using MSS.API.Common;

namespace MSS.API.Model.Data
{
    public class UploadFile
    {
        public int ID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public int SystemResource { get; set; }
        public string SystemResourceName { get; set; }

    }

    public class UploadFileMap : EntityMap<UploadFile>
    {
        public UploadFileMap()
        {
            Map(o => o.FileName).ToColumn("file_name");
            Map(o => o.FilePath).ToColumn("file_path");
            Map(o => o.SystemResource).ToColumn("system_resource");
            Map(o => o.SystemResourceName).ToColumn("resourceName");
            Map(o => o.Type).ToColumn("type");
            Map(o => o.TypeName).ToColumn("name");
        }
    }
}
