using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Model.DTO
{
    public class UserOperationLogParm : BaseQueryParm
    {
        /// <summary>
        /// group_name模糊查询
        /// </summary>
        public string actionName { get; set; }
    }
}
