using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Core.Common;
using MSS.API.Model.DTO;
using MSS.API.Common;

namespace MSS.API.Core.V1.Business
{
    public interface IExpertDataService
    {
        Task<ApiResult> Add(tb_expert_data model);
        Task<ApiResult> Update(tb_expert_data node);
        Task<ApiResult> Delete(tb_expert_data node);

        Task<ApiResult> DeleteList(string ids); 

        Task<ApiResult> Exists(int id);
        Task<ApiResult> GetModel(int id);

       // Task<ApiResult> GetRecordCount(string where);

       // Task<ApiResult> GetMaxId();

        // Task<ApiResult> GetList(int id);

        /// <summary>
        /// 组合条件查询所有
        /// </summary>
        /// <param name="strWhere"></param>
        /// <returns></returns>
        // Task<ApiResult> GetList(string strWhere);

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="strWhere">查询条件</param>
        /// <param name="orderby">asc：desc</param>
        /// <param name="sort">分类字段</param>
        /// <param name="pagesize">每页大小</param>
        /// <returns></returns>
        Task<ApiResult> GetListByPage(string strWhere,  string sort, string orderby, int page, int size);
    }
}
