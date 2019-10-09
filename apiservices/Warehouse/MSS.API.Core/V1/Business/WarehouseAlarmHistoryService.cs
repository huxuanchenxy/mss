using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using MSS.API.Model.DTO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MSS.API.Common;
using static MSS.API.Common.Const;
using static MSS.API.Common.FilePath;
using Microsoft.AspNetCore.Http;
using System.IO;
using MSS.API.Common.Utility;

namespace MSS.API.Core.V1.Business
{
    public class WarehouseAlarmHistoryService : IWarehouseAlarmHistoryService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IWarehouseAlarmHistoryRepo<WarehouseAlarmHistory> _warehouseAlarmHistoryRepo;

        private readonly int userID;
        public WarehouseAlarmHistoryService(IWarehouseAlarmHistoryRepo<WarehouseAlarmHistory> warehouseAlarmHistoryRepo, IAuthHelper auth)
        {
            //_logger = logger;
            _warehouseAlarmHistoryRepo = warehouseAlarmHistoryRepo;
            userID = auth.GetUserId();
        }

        public async Task<ApiResult> GetPageByParm(WarehouseAlarmHistoryQueryParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows = parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                ret.data = await _warehouseAlarmHistoryRepo.GetPageByParm(parm);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

    }
}
