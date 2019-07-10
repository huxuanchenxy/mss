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
    public interface IDeviceMaintainRegService
    {
        Task<ApiResult> Add(tb_devicemaintain_reg model);
        Task<ApiResult> Update(tb_devicemaintain_reg node);
        Task<ApiResult> Delete(tb_devicemaintain_reg node);

        Task<ApiResult> DeleteList(string ids);

        Task<ApiResult> Exists(int id);
        Task<ApiResult> GetModel(int id);

     

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="strWhere">查询条件</param>
        /// <param name="orderby">asc：desc</param>
        /// <param name="sort">分类字段</param>
        /// <param name="pagesize">每页大小</param>
        /// <returns></returns>
        Task<ApiResult> GetListByPage(string strWhere, string sort, string orderby, int page, int size);
    }
}
