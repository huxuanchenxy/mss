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
        private readonly IStockOperationRepo<StockOperation> _stockOperationRepo;
        private readonly IWarehouseAlarmHistoryRepo<WarehouseAlarmHistory> _warehouseAlarmHistoryRepo;

        private readonly int userID;
        public WarehouseAlarmService(IWarehouseAlarmRepo<WarehouseAlarm> warehouseAlarmRepo,
            IWarehouseAlarmHistoryRepo<WarehouseAlarmHistory> warehouseAlarmHistoryRepo,
            IStockOperationRepo<StockOperation> stockOperationRepo, IAuthHelper auth)
        {
            //_logger = logger;
            _warehouseAlarmRepo = warehouseAlarmRepo;
            _stockOperationRepo = stockOperationRepo;
            _warehouseAlarmHistoryRepo = warehouseAlarmHistoryRepo;
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
                List<int> sp = new List<int>() { warehouseAlarm.SpareParts };
                List<WarehouseAlarm> was= await _warehouseAlarmRepo.GetBySPsAndWH(sp, warehouseAlarm.Warehouse);
                if (was.Count>0)
                {
                    ret.code = Code.DataIsExist;
                    ret.msg = "已存在相同库存和物资的预警设置";
                    return ret;
                }
                WarehouseAlarm w = await GetIsAlarm(warehouseAlarm);
                using (TransactionScope scope = new TransactionScope())
                {
                    ret.data = await _warehouseAlarmRepo.Save(warehouseAlarm);
                    if (w.IsStockSumUpdate) await _stockOperationRepo.UpdateStockSumAlarm(sp, w.IsStockSumAlarm);
                    if (w.IsStockUpdate) await _stockOperationRepo.UpdateStockAlarm(sp, w.IsAlarm);
                    if (w.WAlarmHistory != null) await _warehouseAlarmHistoryRepo.Save(w.WAlarmHistory);
                    scope.Complete();
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

        public async Task<ApiResult> Update(WarehouseAlarm warehouseAlarm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                WarehouseAlarm et = await _warehouseAlarmRepo.GetByID(warehouseAlarm.ID);
                if (et!=null)
                {
                    List<int> sp = new List<int>() { warehouseAlarm.SpareParts };
                    if (et.Warehouse != warehouseAlarm.Warehouse || et.SpareParts != warehouseAlarm.SpareParts)
                    {
                        List<WarehouseAlarm> was = await _warehouseAlarmRepo.GetBySPsAndWH(sp, warehouseAlarm.Warehouse);
                        if (was.Count > 0)
                        {
                            ret.code = Code.DataIsExist;
                            ret.msg = "已存在相同库存和物资的预警设置";
                            return ret;
                        }
                    }
                    DateTime dt = DateTime.Now;
                    warehouseAlarm.UpdatedTime = dt;
                    warehouseAlarm.UpdatedBy = userID;
                    if (et.SafeStorage != warehouseAlarm.SafeStorage)
                    {
                        WarehouseAlarm w = await GetIsAlarm(warehouseAlarm);
                        using (TransactionScope scope = new TransactionScope())
                        {
                            ret.data = await _warehouseAlarmRepo.Update(warehouseAlarm);
                            if (w.IsStockSumUpdate) await _stockOperationRepo.UpdateStockSumAlarm(sp, w.IsStockSumAlarm);
                            if (w.IsStockUpdate) await _stockOperationRepo.UpdateStockAlarm(sp, w.IsAlarm);
                            if (w.WAlarmHistory != null) await _warehouseAlarmHistoryRepo.Save(w.WAlarmHistory);
                            scope.Complete();
                        }
                    }
                    else
                    {
                        ret.data = await _warehouseAlarmRepo.Update(warehouseAlarm);
                    }
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
                string[] arr = ids.Split(',');
                List<int> list = arr.Select(a=>Convert.ToInt32(a)).ToList();
                using (TransactionScope scope = new TransactionScope())
                {
                    ret.data = await _warehouseAlarmRepo.Delete(arr);
                    await _stockOperationRepo.UpdateStockAlarm(list, 0);
                    await _stockOperationRepo.UpdateStockSumAlarm(list, 0);
                    scope.Complete();
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

        private async Task<WarehouseAlarm> GetIsAlarm(WarehouseAlarm warehouseAlarm)
        {
            WarehouseAlarm ret = warehouseAlarm;
            List<Stock> ss = await _stockOperationRepo.ListBySPsAndWH(new List<int>() { ret.SpareParts }, ret.Warehouse);
            List<Stock> ssAll = await _stockOperationRepo.ListStockBySPs(new List<int>() { ret.SpareParts });
            List<StockSum> sss = await _stockOperationRepo.ListBySPs(new List<int>() { ret.SpareParts });
            ret.IsAlarm = 0;
            if (ss.Count > 0)
            {
                if (ss[0].StockNo < ret.SafeStorage) ret.IsAlarm = 1;
                if (ss[0].IsAlarm != ret.IsAlarm)
                {
                    ret.IsStockUpdate = true;
                    if (ret.IsAlarm == 1)
                    {
                        ret.WAlarmHistory = new WarehouseAlarmHistory();
                        ret.WAlarmHistory.SpareParts = ret.SpareParts;
                        ret.WAlarmHistory.SafeStorage = ret.SafeStorage;
                        ret.WAlarmHistory.StockNo = ss[0].StockNo;
                        ret.WAlarmHistory.Warehouse = ret.Warehouse;
                        ret.WAlarmHistory.CreatedTime = DateTime.Now;
                    }
                }
                if (ret.IsAlarm == 1 || ssAll.Where(a => a.IsAlarm == 1).Count() > 1)
                {
                    if (sss[0].IsAlarm==0)
                    {
                        ret.IsStockSumUpdate = true;
                        ret.IsStockSumAlarm = 1;
                    }
                }
                else
                {
                    if (sss[0].IsAlarm == 1)
                    {
                        ret.IsStockSumUpdate = true;
                        ret.IsStockSumAlarm = 0;
                    }
                }
            }
            return ret;
        }
    }
}
