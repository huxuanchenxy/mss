using System;
using System.Collections.Generic;
using System.Text;
using Dapper.FluentMap.Mapping;
namespace MSS.API.Model.Data
{
    public class User:BaseEntity
    {
        public string AccountName { get; set; }
        public string UserName { get; set; }
        public string Mobile {get; set;}
        public int Sex { get; set; }
    }

    public class UserMap : EntityMap<User>
    {
        public UserMap()
        {
            Map(o => o.AccountName).ToColumn("acc_name");
            Map(o => o.UserName).ToColumn("user_name");
            Map(o => o.Mobile).ToColumn("mobile");
            Map(o => o.Sex).ToColumn("sex");

            Map(o => o.CreatedBy).ToColumn("created_by");
            Map(o => o.CreatedTime).ToColumn("created_time");
            Map(o => o.UpdatedBy).ToColumn("updated_by");
            Map(o => o.UpdatedTime).ToColumn("updated_time");
            Map(o => o.IsDel).ToColumn("is_del");
        }
    }
}
