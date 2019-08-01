using MSS.API.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSS.API.Core.V1.Business
{
    public interface ILifeTimeKeyMaintainService
    {
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="strWhere">查询条件</param>
        /// <param name="orderby">asc：desc</param>
        /// <param name="sort">分类字段</param>
        /// <param name="pagesize">每页大小</param>
        /// <returns></returns>
        Task<ApiResult> GetListByPage(string strWhere, string sort, string orderby, int page, int size); 

        Task<ApiResult> GetdeviceList(string strWhere); 
    }
}
