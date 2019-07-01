using System;
using System.Collections.Generic;
using System.Text;

using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public abstract class BaseEntity
    {
        public int ID { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedTime { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedTime { get; set; }
        public int Is_deleted { get; set;}
    }

    public class BaseEntityMap : EntityMap<BaseEntity>
    {
        public BaseEntityMap()
        {
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.Is_deleted).ToColumn("Is_deleted");
        }
    }
}
