using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class EquipmentType:BaseEntity
    {
        public string TName { get; set; }
        public string Model { get; set; }
        public string Desc { get; set; }
        public string UploadFiles { get; set; }
        public string CreatedName { get; set; }
        public string UpdatedName { get; set; }
    }
    public class EquipmentTypeMap : EntityMap<EquipmentType>
    {
        public EquipmentTypeMap()
        {
            Map(o => o.TName).ToColumn("type_name");
            Map(o => o.Model).ToColumn("model");
            Map(o => o.Desc).ToColumn("description");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedName).ToColumn("created_name");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedName).ToColumn("updated_name");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }

    public class EqpTypeQueryParm:BaseQueryParm
    {
        public string SearchName { get; set; }
        public string SearchDesc { get; set; }
    }

    public class EqpTypeView
    {
        public List<EquipmentType> rows { get; set; }
        public int total { get; set; }
    }
}
