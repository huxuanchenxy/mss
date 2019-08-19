using System.Collections.Generic;
namespace MSS.API.Dao.Common
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