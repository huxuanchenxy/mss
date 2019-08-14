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
    public class WarehouseService : IWarehouseService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IWarehouseRepo<Warehouse> _warehouseRepo;

        private readonly int userID;
        public WarehouseService(IWarehouseRepo<Warehouse> warehouseRepo, IAuthHelper auth)
        {
            //_logger = logger;
            _warehouseRepo = warehouseRepo;
            userID = auth.GetUserId();
        }

        public async Task<ApiResult> Save(Warehouse warehouse)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                warehouse.UpdatedTime = dt;
                warehouse.CreatedTime = dt;
                warehouse.UpdatedBy = userID;
                warehouse.CreatedBy = userID;
                ret.data = await _warehouseRepo.Save(warehouse);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> Update(Warehouse warehouse)
        {
            ApiResult ret = new ApiResult();
            try
            {
                Warehouse et = await _warehouseRepo.GetByID(warehouse.ID);
                if (et!=null)
                {
                    DateTime dt = DateTime.Now;
                    warehouse.UpdatedTime = dt;
                    warehouse.UpdatedBy = userID;
                    ret.data = await _warehouseRepo.Update(warehouse);
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
                // 判断设备类型下有没有挂设备，有的话不允许删除
                ret.data = await _warehouseRepo.Delete(ids.Split(','),userID);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> GetPageByParm(WarehouseQueryParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows = parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                ret.data = await _warehouseRepo.GetPageByParm(parm);
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
                ret.data = await _warehouseRepo.GetByID(id);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> GetAll()
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _warehouseRepo.GetAll();
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
