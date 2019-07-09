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
        public int FileType { get; set; }
        public int ForeignID { get; set; }
    }

    public class UploadFileMap : EntityMap<UploadFile>
    {
        public UploadFileMap()
        {
            Map(o => o.FileName).ToColumn("file_name");
            Map(o => o.FilePath).ToColumn("file_path");
            Map(o => o.FileType).ToColumn("type");
            Map(o => o.ForeignID).ToColumn("foreign_id");
        }
    }
}
