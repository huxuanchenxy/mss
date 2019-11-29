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
    public interface IStorageLocationService
    {
        Task<ApiResult> Save(StorageLocation storageLocation);
        Task<ApiResult> Update(StorageLocation storageLocation);
        Task<ApiResult> Delete(string ids);
        Task<ApiResult> GetPageByParm(StorageLocationQueryParm parm);
        Task<ApiResult> GetByID(int id);
        Task<ApiResult> GetAll();
        Task<ApiResult> ListByWarehouse(int warehouse);
        Task<ApiResult> ListWarehyouseLocation();
    }

    public class StorageLocationService : IStorageLocationService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IStorageLocationRepo<StorageLocation> _storageLocationRepo;
        //private readonly IWarehouseAlarmRepo<WarehouseAlarm> _warehouseAlarmRepo;
        //private readonly IStockOperationRepo<StockOperation> _stockOperationRepo;

        private readonly int userID;
        public StorageLocationService(IStorageLocationRepo<StorageLocation> storageLocationRepo, IAuthHelper auth,
            IWarehouseAlarmRepo<WarehouseAlarm> warehouseAlarmRepo,
            IStockOperationRepo<StockOperation> stockOperationRepo)
        {
            //_logger = logger;
            _storageLocationRepo = storageLocationRepo;
            //_stockOperationRepo = stockOperationRepo;
            //_warehouseAlarmRepo = warehouseAlarmRepo;
            userID = auth.GetUserId();
        }

        public async Task<ApiResult> Save(StorageLocation storageLocation)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                storageLocation.UpdatedTime = dt;
                storageLocation.CreatedTime = dt;
                storageLocation.UpdatedBy = userID;
                storageLocation.CreatedBy = userID;
                ret.data = await _storageLocationRepo.Save(storageLocation);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> Update(StorageLocation storageLocation)
        {
            ApiResult ret = new ApiResult();
            try
            {
                StorageLocation et = await _storageLocationRepo.GetByID(storageLocation.ID);
                if (et!=null)
                {
                    DateTime dt = DateTime.Now;
                    storageLocation.UpdatedTime = dt;
                    storageLocation.UpdatedBy = userID;
                    ret.data = await _storageLocationRepo.Update(storageLocation);
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
                ret.data = await _storageLocationRepo.Delete(tmp, userID);
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResult> GetPageByParm(StorageLocationQueryParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows = parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                ret.data = await _storageLocationRepo.GetPageByParm(parm);
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
                ret.data = await _storageLocationRepo.GetByID(id);
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
                ret.data = await _storageLocationRepo.GetAll();
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> ListByWarehouse(int warehouse)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _storageLocationRepo.ListByWarehouse(warehouse);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> ListWarehyouseLocation()
        {
            ApiResult ret = new ApiResult();
            List<ElementUICascader> uis = new List<ElementUICascader>();
            try
            {
                List<StorageLocation> sls= await _storageLocationRepo.GetAll();
                IEnumerable<IGrouping<int, StorageLocation>> groupSL = sls.GroupBy(a => a.Warehouse);
                foreach (IGrouping<int, StorageLocation> group in groupSL)
                {
                    ElementUICascader ui = new ElementUICascader();
                    ui.children = new List<ElementUICascader>();
                    ui.value = group.Key;
                    var tmp = group.FirstOrDefault();
                    ui.label = tmp.WarehouseName;
                    foreach (StorageLocation sl in group.OrderBy(g => g.Name))
                    {
                        ElementUICascader ui1 = new ElementUICascader();
                        ui1.value = sl.ID;
                        ui1.label = sl.Name;
                        ui.children.Add(ui1);
                    }
                    uis.Add(ui);
                }
                ret.data = uis;
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
