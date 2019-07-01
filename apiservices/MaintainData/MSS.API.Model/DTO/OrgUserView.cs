using System;
using System.Collections.Generic;
using System.Text;
using Dapper.FluentMap.Mapping;
using MSS.API.Model.Data;
namespace MSS.API.Model.DTO
{
    public class OrgUserView : BaseEntity
    {
        public List<int> UserIDs { get; set; }
    }
}
