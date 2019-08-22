using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Common;
using MSS.API.Model.DTO;
using Microsoft.AspNetCore.Http;

namespace MSS.API.Core.V1.Business
{
    public interface IWarehouseAlarmService
    {
        Task<ApiResult> Save(WarehouseAlarm warehouseAlarm);
        Task<ApiResult> Update(WarehouseAlarm warehouseAlarm);
        Task<ApiResult> Delete(string ids);
        Task<ApiResult> GetPageByParm(WarehouseAlarmQueryParm parm);
        Task<ApiResult> GetByID(int id);
    }
}
