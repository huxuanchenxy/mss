using System;
using System.Collections.Generic;
using System.Text;

namespace System.API.DTO
{
    public class BigAreaQueryParm : BaseQueryParm
    {
        /// <summary>
        ///查询条件
        /// </summary>
        public string SearchName { get; set; }
        public string searchType { get; set; }
        
    }
 
    
    public class MidAreaQueryParm : BaseQueryParm
    {
        /// <summary>
        ///查询条件
        /// </summary>
        public string SearchName { get; set; }

        public string searchType { get; set; }
    }

}
