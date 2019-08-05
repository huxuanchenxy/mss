using System;
using System.Collections.Generic;
using System.Text;

namespace MSS.API.Core.Common
{
    public class PageData<T>
    {
        /// <summary>
        /// 查询结果集
        /// </summary>
        public List<T> rows { get; set; }
        /// <summary>
        /// 总数
        /// </summary>
        public int total { get; set; }
    }
}
