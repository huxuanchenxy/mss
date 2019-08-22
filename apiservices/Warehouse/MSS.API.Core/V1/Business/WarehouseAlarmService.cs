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
    public class WarehouseAlarmService : IWarehouseAlarmService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IWarehouseAlarmRepo<WarehouseAlarm> _warehouseAlarmRepo;

        private readonly int userID;
        public WarehouseAlarmService(IWarehouseAlarmRepo<WarehouseAlarm> warehouseAlarmRepo, IAuthHelper auth)
        {
            //_logger = logger;
            _warehouseAlarmRepo = warehouseAlarmRepo;
            userID = auth.GetUserId();
        }

        public async Task<ApiResult> Save(WarehouseAlarm warehouseAlarm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                warehouseAlarm.UpdatedTime = dt;
                warehouseAlarm.CreatedTime = dt;
                warehouseAlarm.UpdatedBy = userID;
                warehouseAlarm.CreatedBy = userID;
                ret.data = await _warehouseAlarmRepo.Save(warehouseAlarm);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> Update(WarehouseAlarm warehouseAlarm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                WarehouseAlarm et = await _warehouseAlarmRepo.GetByID(warehouseAlarm.ID);
                if (et!=null)
                {
                    DateTime dt = DateTime.Now;
                    warehouseAlarm.UpdatedTime = dt;
                    warehouseAlarm.UpdatedBy = userID;
                    ret.data = await _warehouseAlarmRepo.Update(warehouseAlarm);
                }
                else
                {
                    ret.code = Code.DataIsnotExist;
                    ret.msg = "所要修改的数据不存在";
                }
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> Delete(string ids)
        {
            ApiResult ret = new ApiResult();
            try
            {
                // 有关联的物资不允许删除
                ret.data = await _warehouseAlarmRepo.Delete(ids.Split(','));
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> GetPageByParm(WarehouseAlarmQueryParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows = parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                ret.data = await _warehouseAlarmRepo.GetPageByParm(parm);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> GetByID(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _warehouseAlarmRepo.GetByID(id);
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
