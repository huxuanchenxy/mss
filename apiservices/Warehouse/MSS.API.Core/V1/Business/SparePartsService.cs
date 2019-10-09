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
    public class SparePartsService : ISparePartsService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly ISparePartsRepo<SpareParts> _sparePartsRepo;
        private readonly IWarehouseAlarmRepo<WarehouseAlarm> _warehouseAlarmRepo;
        private readonly IStockOperationRepo<StockOperation> _stockOperationRepo;

        private readonly int userID;
        public SparePartsService(ISparePartsRepo<SpareParts> sparePartsRepo, IAuthHelper auth,
            IWarehouseAlarmRepo<WarehouseAlarm> warehouseAlarmRepo,
            IStockOperationRepo<StockOperation> stockOperationRepo)
        {
            //_logger = logger;
            _sparePartsRepo = sparePartsRepo;
            _stockOperationRepo = stockOperationRepo;
            _warehouseAlarmRepo = warehouseAlarmRepo;
            userID = auth.GetUserId();
        }

        public async Task<ApiResult> Save(SpareParts spareParts)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                spareParts.UpdatedTime = dt;
                spareParts.CreatedTime = dt;
                spareParts.UpdatedBy = userID;
                spareParts.CreatedBy = userID;
                ret.data = await _sparePartsRepo.Save(spareParts);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> Update(SpareParts spareParts)
        {
            ApiResult ret = new ApiResult();
            try
            {
                SpareParts et = await _sparePartsRepo.GetByID(spareParts.ID);
                if (et!=null)
                {
                    DateTime dt = DateTime.Now;
                    spareParts.UpdatedTime = dt;
                    spareParts.UpdatedBy = userID;
                    ret.data = await _sparePartsRepo.Update(spareParts);
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
                string[] tmp = ids.Split(',');
                // 有关联的物资不允许删除
                if (await _stockOperationRepo.hasSpareParts(tmp))
                {
                    ret.code = Code.DataIsExist;
                    ret.msg = "仓库中已有此物资，不可删除";
                }
                else
                {
                    using (TransactionScope scope = new TransactionScope())
                    {
                        ret.data = await _sparePartsRepo.Delete(tmp, userID);
                        await _warehouseAlarmRepo.DeleteBySPs(tmp);
                        scope.Complete();
                    }
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

        public async Task<ApiResult> GetPageByParm(SparePartsQueryParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows = parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                ret.data = await _sparePartsRepo.GetPageByParm(parm);
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
                ret.data = await _sparePartsRepo.GetByID(id);
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
                ret.data = await _sparePartsRepo.GetAll();
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
