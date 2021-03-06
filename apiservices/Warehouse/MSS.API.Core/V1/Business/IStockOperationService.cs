﻿using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Common;
using MSS.API.Model.DTO;
using Microsoft.AspNetCore.Http;
using static MSS.API.Model.Data.Common;

namespace MSS.API.Core.V1.Business
{
    public interface IStockOperationService
    {
        Task<ApiResult> Save(StockOperation stockOperation);
        Task<ApiResult> ListByReason(int reason);
        Task<ApiResult> ListByOperation(int operation);

        Task<ApiResult> GetPageByParm(StockOperationQueryParm parm);
        Task<ApiResult> GetByID(int id,bool isEdit=false);

        Task<ApiResult> GetStockSumPageByParm(StockSumQueryParm parm);

        Task<ApiResult> ListStockDetailBySPsAndWH(int spareParts, int warehouse, 
            StockOptDetailType reason,int storageLocation);
        Task<ApiResult> GetStockDetailByID(int id);

        Task<ApiResult> ListByWH(int warehouse);
        Task<ApiResult> ListStockDetail(int reason);
    }
}
