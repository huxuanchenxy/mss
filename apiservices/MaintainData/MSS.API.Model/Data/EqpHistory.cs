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
    public class EqpHistory
    {
        public int ID { get; set; }
        public int EqpID { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; }
        public int WorkingOrder { get; set; }
        public DateTime CreatedTime { get; set; }
        public string CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }

    /// <summary>
    /// model
    /// </summary>
    public class EqpHistoryMap : EntityMap<EqpHistory>
    {
        public EqpHistoryMap()
        {
            Map(o => o.ID).ToColumn("id");
            Map(o => o.EqpID).ToColumn("eqp");
            Map(o => o.Type).ToColumn("type");
            Map(o => o.TypeName).ToColumn("name");
            Map(o => o.WorkingOrder).ToColumn("working_order");
            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.CreatedDate).ToColumn("created_date");
        }
    }

}