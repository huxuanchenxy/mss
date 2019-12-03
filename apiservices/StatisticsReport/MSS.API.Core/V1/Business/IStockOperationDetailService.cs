using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Common;
using MSS.API.Model.DTO;
namespace MSS.API.Core.V1.Business
{
    public interface IStockOperationDetailService
    {
        Task<ApiResult> GetStockOperationChart(StockOperationDetailParm parm);
        Task<ApiResult> Save(StockOperationDetail obj);
        Task<ApiResult> Update(StockOperationDetail obj);
        Task<ApiResult> Delete(string ids);
        Task<ApiResult> GetByID(int id);
    }
}
