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
using Microsoft.Extensions.Caching.Distributed;
using static MSS.API.Model.Data.Common;

namespace MSS.API.Core.V1.Business
{
    public class StockOperationService : IStockOperationService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IStockOperationRepo<StockOperation> _stockOperationRepo;
        private readonly IWarehouseAlarmRepo<WarehouseAlarm> _warehouseAlarmRepo;
        private readonly IWarehouseAlarmHistoryRepo<WarehouseAlarmHistory> _warehouseAlarmHistoryRepo;
        private readonly IDistributedCache _cache;

        private readonly int userID;
        private ApiResult validateSaveData=new ApiResult();
        public StockOperationService(IStockOperationRepo<StockOperation> stockOperationRepo,
            IWarehouseAlarmHistoryRepo<WarehouseAlarmHistory> warehouseAlarmHistoryRepo,
            IWarehouseAlarmRepo<WarehouseAlarm> warehouseAlarmRepo, IAuthHelper auth, IDistributedCache cache)
        {
            //_logger = logger;
            _stockOperationRepo = stockOperationRepo;
            _warehouseAlarmRepo = warehouseAlarmRepo;
            _warehouseAlarmHistoryRepo = warehouseAlarmHistoryRepo;
            _cache = cache;
            userID = auth.GetUserId();
        }
        #region 库存操作保存
        public async Task<ApiResult> Save(StockOperation stockOperation)
        {
            ApiResult ret = new ApiResult();
            StockOperationSaveParm parm = new StockOperationSaveParm();
            try
            {
                DateTime dt = DateTime.Now;
                stockOperation.CreatedTime = dt;
                stockOperation.CreatedBy = userID;
                StockOperationType t = (StockOperationType)stockOperation.Type;
                string redisKey = GetRedisKey(t);
                long tmp = Convert.ToInt64(_cache.GetString(redisKey)) + 1;
                stockOperation.OperationID = GetOperationID(tmp, t);
                parm.stockOperation = stockOperation;
                int intTmp=0;
                StockOperationSaveParm sosp = new StockOperationSaveParm();
                //构造存货明细表、库存明细表、库存总表
                switch (t)
                {
                    case StockOperationType.Receive:
                        switch ((StockOptDetailType)stockOperation.Reason)
                        {
                            case StockOptDetailType.PurchaseReceive:
                            case StockOptDetailType.OtherReceive:
                                sosp = await GetParmInit(parm);
                                if (validateSaveData.code != Code.Success) return validateSaveData;
                                intTmp = await _stockOperationRepo.Save(sosp);
                                break;
                            case StockOptDetailType.PurchaseReturn:
                                sosp = await GetParm(
                                    StockChange.Subtract, StockChange.NoChange, StockChange.Subtract, StockChange.NoChange,
                                    StockChange.NoChange, StockChange.NoChange, StockChange.NoChange, StockStatus.Returned, parm);
                                if (validateSaveData.code != Code.Success) return validateSaveData;
                                intTmp = await _stockOperationRepo.Save(sosp);
                                break;
                        }
                        break;
                    case StockOperationType.Delivery:
                        switch ((StockOptDetailType)stockOperation.Reason)
                        {
                            case StockOptDetailType.Distribution:
                                sosp = await GetParm(
                                    StockChange.Subtract, StockChange.NoChange, StockChange.Subtract, StockChange.NoChange,
                                    StockChange.NoChange, StockChange.NoChange, StockChange.NoChange, StockStatus.Normal, parm);
                                if (validateSaveData.code != Code.Success) return validateSaveData;
                                intTmp = await _stockOperationRepo.Save(sosp);
                                break;
                            case StockOptDetailType.MaterialReturn:
                                sosp = await GetParm(
                                    StockChange.Plus, StockChange.NoChange, StockChange.Plus,StockChange.NoChange,
                                    StockChange.NoChange, StockChange.NoChange, StockChange.NoChange, StockStatus.Normal, 
                                    parm);
                                if (validateSaveData.code != Code.Success) return validateSaveData;
                                intTmp = await _stockOperationRepo.Save(sosp);
                                break;
                            case StockOptDetailType.TroubleReturn:
                                sosp = await GetParm(
                                    StockChange.Plus, StockChange.Plus, StockChange.NoChange,StockChange.NoChange,
                                    StockChange.NoChange, StockChange.NoChange, StockChange.NoChange, StockStatus.Trouble, 
                                    parm);
                                if (validateSaveData.code != Code.Success) return validateSaveData;
                                intTmp = await _stockOperationRepo.Save(sosp);
                                break;
                            case StockOptDetailType.TroubleRepair:
                                sosp = await GetParm(
                                    StockChange.NoChange, StockChange.Subtract, StockChange.NoChange,StockChange.NoChange,
                                    StockChange.Plus, StockChange.NoChange, StockChange.NoChange, StockStatus.Repair, parm);
                                if (validateSaveData.code != Code.Success) return validateSaveData;
                                intTmp = await _stockOperationRepo.Save(sosp);
                                break;
                            case StockOptDetailType.MaterialLend:
                                sosp = await GetParm(
                                    StockChange.NoChange, StockChange.NoChange, StockChange.Subtract,StockChange.NoChange,
                                    StockChange.NoChange, StockChange.Plus, StockChange.NoChange, StockStatus.Lend, parm);
                                if (validateSaveData.code != Code.Success) return validateSaveData;
                                intTmp = await _stockOperationRepo.Save(sosp);
                                break;
                            case StockOptDetailType.TroubleScrap:
                                parm.stockOperation.Picker = userID;
                                sosp = await GetParm(
                                    StockChange.Subtract, StockChange.Subtract, StockChange.NoChange,StockChange.NoChange,
                                    StockChange.NoChange, StockChange.NoChange, StockChange.Plus, StockStatus.Scrap, parm);
                                if (validateSaveData.code != Code.Success) return validateSaveData;
                                intTmp = await _stockOperationRepo.Save(sosp);
                                break;
                            case StockOptDetailType.InStockScrap:
                                parm.stockOperation.Picker = userID;
                                sosp = await GetParm(
                                    StockChange.Subtract, StockChange.NoChange, StockChange.Subtract, StockChange.NoChange,
                                    StockChange.NoChange, StockChange.NoChange, StockChange.Plus, StockStatus.Scrap, parm);
                                if (validateSaveData.code != Code.Success) return validateSaveData;
                                intTmp = await _stockOperationRepo.Save(sosp);
                                break;
                            case StockOptDetailType.lendReturn:
                                sosp = await GetParmReturn(
                                    StockChange.NoChange, StockChange.NoChange, StockChange.Plus,StockChange.NoChange,
                                    StockChange.NoChange, StockChange.Subtract, StockChange.NoChange, StockStatus.Normal, 
                                    parm, StockOperationStatus.NoReturn);
                                if (validateSaveData.code != Code.Success) return validateSaveData;
                                intTmp = await _stockOperationRepo.Save(sosp);
                                break;
                            case StockOptDetailType.RepairReceive:
                                sosp = await GetParmReturn(
                                    StockChange.NoChange, StockChange.NoChange, StockChange.Plus,StockChange.NoChange,
                                    StockChange.Subtract, StockChange.NoChange, StockChange.NoChange, StockStatus.Normal, 
                                    parm, StockOperationStatus.NoReturn);
                                if (validateSaveData.code != Code.Success) return validateSaveData;
                                intTmp = await _stockOperationRepo.Save(sosp);
                                break;
                            case StockOptDetailType.Inspection:
                                sosp = await GetParm(
                                    StockChange.NoChange, StockChange.NoChange, StockChange.Subtract,StockChange.Plus,
                                    StockChange.NoChange, StockChange.NoChange, StockChange.NoChange, StockStatus.Inspection, parm);
                                if (validateSaveData.code != Code.Success) return validateSaveData;
                                intTmp = await _stockOperationRepo.Save(sosp);
                                break;
                            case StockOptDetailType.InspectionReturn:
                                sosp = await GetParmReturn(
                                    StockChange.NoChange, StockChange.NoChange, StockChange.Plus,StockChange.Subtract,
                                    StockChange.NoChange, StockChange.NoChange, StockChange.NoChange, StockStatus.Normal, 
                                    parm, StockOperationStatus.NoReturn);
                                if (validateSaveData.code != Code.Success) return validateSaveData;
                                intTmp = await _stockOperationRepo.Save(sosp);
                                break;
                        }
                        break;
                    case StockOperationType.Adjust:
                        switch ((StockOptDetailType)stockOperation.Reason)
                        {
                            case StockOptDetailType.InventoryLoss:
                                sosp = await GetParm(
                                    StockChange.Subtract, StockChange.NoChange, StockChange.Subtract, StockChange.NoChange,
                                    StockChange.NoChange, StockChange.NoChange, StockChange.NoChange, StockStatus.Loss,
                                    parm);
                                if (validateSaveData.code != Code.Success) return validateSaveData;
                                intTmp = await _stockOperationRepo.Save(sosp);
                                break;
                            case StockOptDetailType.InventoryProfit:
                                sosp = await GetParmProfit(parm);
                                if (validateSaveData.code != Code.Success) return validateSaveData;
                                intTmp = await _stockOperationRepo.Save(sosp);
                                break;
                        }
                        break;
                    case StockOperationType.Move:
                        sosp = await GetParmMove(parm);
                        if (validateSaveData.code != Code.Success) return validateSaveData;
                        intTmp = await _stockOperationRepo.Save(sosp);
                        break;
                }

                if (intTmp > 0)
                {
                    _cache.SetString(redisKey, tmp.ToString());
                }
                ret.data = intTmp;
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }
        /// <summary>
        /// 只处理操作数据，不关心原始数据，大部分原因都调用此函数
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="trouble"></param>
        /// <param name="inStock"></param>
        /// <param name="inspection"></param>
        /// <param name="repair"></param>
        /// <param name="lent"></param>
        /// <param name="scrap"></param>
        /// <param name="status"></param>
        /// <param name="parm"></param>
        /// <param name="soStatus"></param>
        /// <returns></returns>
        private async Task<StockOperationSaveParm> GetParm(StockChange stock, StockChange trouble, StockChange inStock, StockChange inspection,
            StockChange repair, StockChange lent, StockChange scrap, StockStatus status, StockOperationSaveParm parm,
            StockOperationStatus soStatus = StockOperationStatus.Other)
        {
            StockOperationSaveParm ret = new StockOperationSaveParm();
            StockOperation so = parm.stockOperation;
            ret.stockOperation = so;
            StockOptDetailType reason = (StockOptDetailType)so.Reason;
            List<StockOperationDetail> sods = JsonConvert.DeserializeObject<List<StockOperationDetail>>(so.DetailList).OrderBy(a => a.StockDetail).ToList();
            List<int> storageLocations = sods.Select(a => a.StorageLocation).Distinct().ToList();
            ret.stockOperationDetails = sods.OrderBy(a => a.OrderNo).ToList();
            // 获取原始出库数量以及已归(退)还数量
            List<StockOperationDetail> initSods = (await _stockOperationRepo.ListSODByIDs(
                sods.Select(a => a.FromStockOperationDetail).ToList())).OrderBy(a => a.StockDetail).ToList();
            List<StockOperationDetail> initSodsInfo = new List<StockOperationDetail>();
            if (reason == StockOptDetailType.TroubleReturn || reason == StockOptDetailType.MaterialReturn)
            {
                // 获取原始数据的汇率、质保期、供应商等信息
                initSodsInfo = (await _stockOperationRepo.GetByOperationDetail(
                    sods.Select(a => a.FromStockOperationDetail).ToList())).ToList();
            }
            // 获取存货明细
            List<StockDetail> initSds = (await _stockOperationRepo.ListStockDetailByIDs(
                sods.Select(a => a.StockDetail).ToList()
                )).OrderBy(a => a.ID).ToList();
            ret.stockDetails = new List<StockDetail>();
            ret.stockDetailsUpdate = new List<StockDetail>();
            ret.stocks = new List<Stock>();
            ret.stockSums = new List<StockSum>();
            ret.wAlarmHistory = new List<WarehouseAlarmHistory>();
            List<int> sps = sods.Select(a => a.SpareParts).Distinct().ToList();
            List<WarehouseAlarm> was = await _warehouseAlarmRepo.GetBySPsAndWH(sps, so.Warehouse);
            List<Stock> stocksTmp = await _stockOperationRepo.ListBySPsAndWH(sps, so.Warehouse, storageLocations);
            List<Stock> stocksTmpAll = await _stockOperationRepo.ListStockBySPs(sps);
            List<StockSum> stockSumsTmp = await _stockOperationRepo.ListBySPs(sps.ToList());
            // 归(退)还时，更新已归(退)还数量
            // 归还时，还需更新状态（未归还/其他）
            if (so.FromStockOperation != null)
            {
                ret.stockOperationDetailsUpdate = new List<StockOperationDetail>();
                for (int i = 0; i < initSods.Count; i++)
                {
                    StockOperationDetail sod = new StockOperationDetail();
                    sod.ReturnNo = initSods[i].ReturnNo + sods[i].CountNo;
                    sod.Status = (int)soStatus;
                    sod.ID = sods[i].FromStockOperationDetail;
                    if (soStatus == StockOperationStatus.NoReturn && sod.ReturnNo == initSods[i].CountNo)
                    {
                        sod.Status = (int)StockOperationStatus.Other;
                    }
                    else if (sod.ReturnNo > initSods[i].CountNo)
                    {
                        GetApiResult("归(退)还数量大于原始出库数量，请刷新重试");
                        return ret;
                    }
                    ret.stockOperationDetailsUpdate.Add(sod);
                }
            }
            // 存货明细数据更新
            for (int i = 0; i < initSds.Count; i++)
            {
                StockDetail sd = new StockDetail();
                if ((reason == StockOptDetailType.MaterialReturn || reason == StockOptDetailType.TroubleReturn)
                    && initSds[i].FromStockOperationDetail != initSds[i].StorageLocation)
                {
                    sd= GetBySod(sods[i],so.Warehouse);
                    if (reason == StockOptDetailType.TroubleReturn)
                    {
                        sd.TroubleNo += sods[i].CountNo;
                        sd.InStockNo -= sods[i].CountNo;
                        sd.Status = (int)StockStatus.Trouble;
                    }
                    if (sods[i].Currency == 0 && initSodsInfo.Count > 0)
                    {
                        //新记录时将货币、供应商、保质期存入，方便按照原有逻辑查询
                        StockOperationDetail initSodTmp = initSodsInfo.Where(a => a.ID == sods[i].FromStockOperationDetail).FirstOrDefault();
                        sods[i].Currency = initSodTmp.Currency;
                        sods[i].Supplier = initSodTmp.Supplier;
                        sods[i].LifeDate = initSodTmp.LifeDate;
                        sods[i].Invoice = initSodTmp.Invoice;
                    }
                    ret.stockDetails.Add(sd);
                }
                else
                {
                    sd.ID = initSds[i].ID;
                    sd.StockNo = GetNo(stock, initSds[i].StockNo, sods[i].CountNo);
                    if (sd.StockNo < 0) { GetApiResult("库存不足，请刷新重试"); return ret; }
                    sd.TroubleNo = GetNo(trouble, initSds[i].TroubleNo, sods[i].CountNo);
                    if (sd.TroubleNo < 0) { GetApiResult("故障件数量小于零，请刷新重试"); return ret; }
                    sd.InStockNo = GetNo(inStock, initSds[i].InStockNo, sods[i].CountNo);
                    if (sd.InStockNo < 0) { GetApiResult("存货不足，请刷新重试"); return ret; }
                    sd.InspectionNo = GetNo(inspection, initSds[i].InspectionNo, sods[i].CountNo);
                    if (sd.InspectionNo < 0) { GetApiResult("送检数量小于零，请刷新重试"); return ret; }
                    sd.RepairNo = GetNo(repair, initSds[i].RepairNo, sods[i].CountNo);
                    if (sd.RepairNo < 0) { GetApiResult("送修数量小于零，请刷新重试"); return ret; }
                    sd.LentNo = GetNo(lent, initSds[i].LentNo, sods[i].CountNo);
                    if (sd.LentNo < 0) { GetApiResult("外借数量小于零，请刷新重试"); return ret; }
                    sd.ScrapNo = GetNo(scrap, initSds[i].ScrapNo, sods[i].CountNo);
                    if (sd.ScrapNo < 0) { GetApiResult("报废数量小于零，请刷新重试"); return ret; }
                    //if (sd.InStockNo == 0) sd.Status = (int)StockStatus.Exhaust;
                    //else if (sd.InStockNo > 1) sd.Status = (int)StockStatus.None;
                    //else sd.Status = (int)status;
                    if (sd.StockNo > 1 && status != StockStatus.Normal) sd.Status = (int)StockStatus.None;
                    else if (sd.StockNo == 0 && status != StockStatus.Scrap && status != StockStatus.Returned) sd.Status = (int)StockStatus.Exhaust;
                    else sd.Status = (int)status;
                    sd.Warehouse = so.Warehouse;
                    sd.StorageLocation = initSds[i].StorageLocation;
                    ret.stockDetailsUpdate.Add(sd);
                }
                
            }
            List<CompsiteKey> ckOld = sods.Select(a => new CompsiteKey
            {
                SpareParts = a.SpareParts,
                StorageLocation = a.StorageLocation
            }).Distinct(new Comparer()).ToList();
            foreach (var item in ckOld)
            {
                // 仓库明细数据更新
                var sod = sods.Where(a => a.SpareParts == item.SpareParts && a.StorageLocation == item.StorageLocation);
                int countNo = sod.Sum(a => a.CountNo);
                //直接在前端换算成人民币
                double amount = sod.Sum(a => a.Amount);
                var sTmp = stocksTmp.Where(a => a.SpareParts == item.SpareParts && a.StockNo!=0
                && a.StorageLocation == item.StorageLocation);
                Stock s = new Stock();
                StockOperationDetail mySod = sod.FirstOrDefault();
                if (sTmp==null || sTmp.Count()==0)
                {
                    s = GetStock(countNo, item.SpareParts, amount, so.Warehouse,item.StorageLocation);
                    if (reason == StockOptDetailType.TroubleReturn)
                    {
                        s.TroubleNo += countNo;
                        s.InStockNo -= countNo;
                    }
                }
                else
                {
                    s = sTmp.FirstOrDefault();
                    s.IsAdd = false;
                    s.StockNo = GetNo(stock, s.StockNo, countNo);
                    s.TroubleNo = GetNo(trouble, s.TroubleNo, countNo);
                    s.InStockNo = GetNo(inStock, s.InStockNo, countNo);
                    s.InspectionNo = GetNo(inspection, s.InspectionNo, countNo);
                    s.RepairNo = GetNo(repair, s.RepairNo, countNo);
                    s.LentNo = GetNo(lent, s.LentNo, countNo);
                    s.ScrapNo = GetNo(scrap, s.ScrapNo, countNo);
                    s.Amount = GetAmount(stock, s.Amount, amount);
                }
                GetWAlarmHistories(so.Warehouse, item.SpareParts, was, ref s, ref ret);
                ret.stocks.Add(s);
            }
            foreach (var item in sods.Select(a=>a.SpareParts).Distinct())
            {
                // 库存总表数据更新
                var sod = sods.Where(a => a.SpareParts == item);
                int countNo = sod.Sum(a => a.CountNo);
                //直接在前端换算成人民币
                double amount = sod.Sum(a => a.Amount);
                StockSum ss = stockSumsTmp.Where(a => a.SpareParts == item && a.StockNo != 0).FirstOrDefault();
                ss.IsAdd = false;
                ss.StockNo = GetNo(stock, ss.StockNo, countNo);
                ss.TroubleNo = GetNo(trouble, ss.TroubleNo, countNo);
                ss.InStockNo = GetNo(inStock, ss.InStockNo, countNo);
                ss.InspectionNo = GetNo(inspection, ss.InspectionNo, countNo);
                ss.RepairNo = GetNo(repair, ss.RepairNo, countNo);
                ss.LentNo = GetNo(lent, ss.LentNo, countNo);
                ss.ScrapNo = GetNo(scrap, ss.ScrapNo, countNo);
                ss.Amount = GetAmount(stock, ss.Amount, amount);
                int isAlarm = ret.stocks.Where(a => a.SpareParts == item && a.IsAlarm == 1).Count() > 0 ? 1 : 0;
                ss.IsAlarm = isAlarm == 1 || stocksTmpAll.Where(a => a.SpareParts == item && a.IsAlarm == 1).Count() > 0 ? 1 : 0;
                ret.stockSums.Add(ss);
            }
            return ret;
        }

        /// <summary>
        /// 需要从原数据扣除，新数据插入/更新，类似于出库再入库的操作
        /// 只有借用、送修、送检归还时调用
        /// </summary>
        /// <param name="stock"></param>
        /// <param name="trouble"></param>
        /// <param name="inStock"></param>
        /// <param name="inspection"></param>
        /// <param name="repair"></param>
        /// <param name="lent"></param>
        /// <param name="scrap"></param>
        /// <param name="status"></param>
        /// <param name="parm"></param>
        /// <param name="soStatus"></param>
        /// <returns></returns>
        private async Task<StockOperationSaveParm> GetParmReturn(StockChange stock, StockChange trouble, StockChange inStock,StockChange inspection,
            StockChange repair, StockChange lent, StockChange scrap, StockStatus status,StockOperationSaveParm parm, 
            StockOperationStatus soStatus= StockOperationStatus.Other)
        {
            StockOperationSaveParm ret = new StockOperationSaveParm();
            StockOperation so = parm.stockOperation;
            ret.stockOperation = so;
            StockOptDetailType reason = (StockOptDetailType)so.Reason;
            List<StockOperationDetail> sods = JsonConvert.DeserializeObject<List<StockOperationDetail>>(so.DetailList).OrderBy(a => a.StockDetail).ToList();
            List<int> storageLocations = sods.Select(a => a.StorageLocation).Distinct().ToList();
            storageLocations.AddRange(sods.Where(a=>a.FromStorageLocation!=null && a.FromStorageLocation!=0)
                .Select(a => (int)a.FromStorageLocation).Distinct());
            ret.stockOperationDetails = sods.OrderBy(a=>a.OrderNo).ToList();
            // 获取原始出库数量以及已归(退)还数量
            List<StockOperationDetail> initSods = (await _stockOperationRepo.ListSODByIDs(
                sods.Select(a => a.FromStockOperationDetail).ToList())).OrderBy(a=>a.StockDetail).ToList();
            // 获取原始数据的汇率、质保期、供应商等信息
            List<StockOperationDetail> initSodsInfo = (await _stockOperationRepo.GetByOperationDetail(
                sods.Select(a => a.FromStockOperationDetail).ToList())).ToList();
            // 获取存货明细
            List<StockDetail> initSds = (await _stockOperationRepo
                .ListStockDetailBySPsAndWH(null,so.Warehouse,true)).ToList();
            ret.stockDetails = new List<StockDetail>();
            ret.stockDetailsUpdate = new List<StockDetail>();
            ret.stocks = new List<Stock>();
            ret.stockSums = new List<StockSum>();
            ret.wAlarmHistory = new List<WarehouseAlarmHistory>();
            List<int> sps = sods.Select(a => a.SpareParts).Distinct().ToList();
            List<WarehouseAlarm> was = await _warehouseAlarmRepo.GetBySPsAndWH(sps, so.Warehouse);
            List<Stock> stocksTmp = await _stockOperationRepo.ListBySPsAndWH(sps, so.Warehouse, storageLocations.Distinct().ToList());
            List<Stock> stocksTmpAll = await _stockOperationRepo.ListStockBySPs(sps);
            List<StockSum> stockSumsTmp = await _stockOperationRepo.ListBySPs(sps.ToList());
            // 批次物资出库部分后，存货为0，再入库不同库位时
            //var hasEntity = sods.Where(a => !string.IsNullOrWhiteSpace(a.NewEntity)).Select(a => a.NewEntity).Distinct();
            //if (hasEntity.Count()>0 && await _stockOperationRepo.HasEntity(hasEntity.ToList()))
            //{
            //    validateSaveData.code = Code.DataIsExist;
            //    validateSaveData.msg = "物资ID中与库存中已有的重复";
            //    return ret;
            //}
            // 归(退)还时，更新已归(退)还数量
            // 归还时，还需更新状态（未归还/其他）
            if (so.FromStockOperation!=null)
            {
                ret.stockOperationDetailsUpdate = new List<StockOperationDetail>();
                for (int i = 0; i < initSods.Count; i++)
                {
                    StockOperationDetail sod = new StockOperationDetail();
                    sod.ReturnNo = initSods[i].ReturnNo + sods[i].CountNo;
                    sod.Status = (int)soStatus;
                    sod.ID = sods[i].FromStockOperationDetail;
                    if (soStatus == StockOperationStatus.NoReturn && sod.ReturnNo == initSods[i].CountNo)
                    {
                        sod.Status = (int)StockOperationStatus.Other;
                    }
                    else if (sod.ReturnNo > initSods[i].CountNo)
                    {
                        GetApiResult("归(退)还数量大于原始出库数量，请刷新重试");
                        return ret;
                    }
                    ret.stockOperationDetailsUpdate.Add(sod);
                }
            }
            
            foreach (var sod in sods)
            {
                // 存货明细数据StockDetail
                StockDetail sd = new StockDetail();
                //var sdsTmp = initSds.Where(a => a.SpareParts == sod.SpareParts
                //&& a.Warehouse == so.Warehouse && a.StorageLocation == sod.StorageLocation
                //&& a.Entity==sod.Entity);
                ////归还时新的entity或者存入新库位,不考虑批量部分归还的情况
                //if (sdsTmp==null || sdsTmp.Count()==0)
                {
                    //新记录
                    sd = GetBySod(sod, so.Warehouse);
                    //老库位的借用/送修/送检数量更新
                    StockDetail oldSd = initSds
                        .Where(a => a.SpareParts == sod.SpareParts && a.Warehouse == so.Warehouse
                        && a.StorageLocation == sod.FromStorageLocation && a.Entity==sod.Entity && a.StockNo!=0)
                        .FirstOrDefault();
                    GetStockDetailByReason(reason, sod.CountNo,ref sd,ref oldSd);
                    if (validateSaveData.code != Code.Success) return ret;
                    if (oldSd!=null) ret.stockDetailsUpdate.Add(oldSd);
                    //新记录时将货币、供应商、保质期存入，方便按照原有逻辑查询
                    StockOperationDetail initSodTmp = initSodsInfo.Where(a => a.ID == sod.FromStockOperationDetail).FirstOrDefault();
                    sod.Currency = initSodTmp.Currency;
                    sod.Supplier = initSodTmp.Supplier;
                    sod.LifeDate = initSodTmp.LifeDate;
                    sod.Invoice = initSodTmp.Invoice;
                }
                // 归还时和出库时同一库位
                // 出库时必走此路
                //else
                //{
                //    StockDetail initSd = sdsTmp.FirstOrDefault();
                //    sd.IsAdd = false;
                //    sd.ID = initSd.ID;
                //    sd.StockNo = GetNo(stock, initSd.StockNo, sod.CountNo);
                //    if (sd.StockNo < 0) { GetApiResult("库存不足，请刷新重试"); return ret; }
                //    sd.TroubleNo = GetNo(trouble, initSd.TroubleNo, sod.CountNo);
                //    if (sd.TroubleNo < 0) { GetApiResult("故障件数量小于零，请刷新重试"); return ret; }
                //    sd.InStockNo = GetNo(inStock, initSd.InStockNo, sod.CountNo);
                //    if (sd.InStockNo < 0) { GetApiResult("存货不足，请刷新重试"); return ret; }
                //    sd.InspectionNo = GetNo(inspection, initSd.InspectionNo, sod.CountNo);
                //    if (sd.InspectionNo < 0) { GetApiResult("送检数量小于零，请刷新重试"); return ret; }
                //    sd.RepairNo = GetNo(repair, initSd.RepairNo, sod.CountNo);
                //    if (sd.RepairNo < 0) { GetApiResult("送修数量小于零，请刷新重试"); return ret; }
                //    sd.LentNo = GetNo(lent, initSd.LentNo, sod.CountNo);
                //    if (sd.LentNo < 0) { GetApiResult("外借数量小于零，请刷新重试"); return ret; }
                //    sd.ScrapNo = GetNo(scrap, initSd.ScrapNo, sod.CountNo);
                //    if (sd.ScrapNo < 0) { GetApiResult("报废数量小于零，请刷新重试"); return ret; }
                //    //if (sd.InStockNo == 0) sd.Status = (int)StockStatus.Exhaust;
                //    //else if (sd.InStockNo > 1) sd.Status = (int)StockStatus.None;
                //    //else sd.Status = (int)status;
                //    if (sd.StockNo > 1 && status != StockStatus.Normal) sd.Status = (int)StockStatus.None;
                //    else if (sd.StockNo == 0 && status != StockStatus.Scrap && status != StockStatus.Returned) sd.Status = (int)StockStatus.Exhaust;
                //    else sd.Status = (int)status;
                //    sd.Warehouse = so.Warehouse;
                //    sd.StorageLocation = initSd.StorageLocation;
                //}
                ret.stockDetails.Add(sd);

                // 仓库明细数据Stock
                Stock s = new Stock();
                //入库
                GetStockUpdate(sod, StockChange.Plus,so.Warehouse,reason,stocksTmp,ref s,ref ret);
                //从哪里出库
                if (sod.FromStorageLocation != null && reason != StockOptDetailType.MaterialLend && reason != StockOptDetailType.TroubleReturn)
                {
                    GetStockUpdate(sod, StockChange.Subtract, so.Warehouse, reason, stocksTmp, ref s, ref ret);
                }
            }
            foreach (var item in sods.Select(a=>a.SpareParts).Distinct())
            {
                // 库存总表数据更新
                var sod = sods.Where(a => a.SpareParts == item);
                int countNo = sod.Sum(a => a.CountNo);
                //直接在前端换算成人民币
                double amount = sod.Sum(a => a.Amount);
                StockSum ss = stockSumsTmp.Where(a => a.SpareParts == item && a.StockNo != 0).FirstOrDefault();
                ss.IsAdd = false;
                ss.StockNo = GetNo(stock, ss.StockNo, countNo);
                ss.TroubleNo = GetNo(trouble, ss.TroubleNo, countNo);
                ss.InStockNo = GetNo(inStock, ss.InStockNo, countNo);
                ss.InspectionNo = GetNo(inspection, ss.InspectionNo, countNo);
                ss.RepairNo = GetNo(repair, ss.RepairNo, countNo);
                ss.LentNo = GetNo(lent, ss.LentNo, countNo);
                ss.ScrapNo = GetNo(scrap, ss.ScrapNo, countNo);
                ss.Amount = GetAmount(stock, ss.Amount, amount);
                int isAlarm = ret.stocks.Where(a => a.SpareParts == item && a.IsAlarm == 1).Count() > 0 ? 1 : 0;
                ss.IsAlarm = isAlarm == 1 || stocksTmpAll.Where(a => a.SpareParts == item && a.IsAlarm == 1).Count() > 0 ? 1 : 0;
                ret.stockSums.Add(ss);
            }
            return ret;
        }

        //移库
        //sum总表不变
        //存货批次移库时，类似领料(更新)->再入库(插入)的操作，批量物资需要新的物资ID
        //仓库库存表stock中，类似领料再入库的操作，只和sparePartsID和warehouseID有关
        private async Task<StockOperationSaveParm> GetParmMove(StockOperationSaveParm parm)
        {
            StockOperationSaveParm ret = new StockOperationSaveParm();
            StockOperation so = parm.stockOperation;
            ret.stockOperation = so;
            StockOptDetailType reason = (StockOptDetailType)so.Reason;
            List<StockOperationDetail> sods = JsonConvert.DeserializeObject<List<StockOperationDetail>>(so.DetailList).OrderBy(a => a.StockDetail).ToList();
            var partBatch = sods.Where(a => !string.IsNullOrWhiteSpace(a.NewEntity) && a.InStockNo != a.CountNo);
            if (await _stockOperationRepo.HasEntity(partBatch.Select(a => a.NewEntity).ToList()))
            {
                validateSaveData.code = Code.DataIsExist;
                validateSaveData.msg = "物资ID中与库存中已有的重复";
                return ret;
            }

            List<int> storageLocations = sods.Select(a => a.StorageLocation).Distinct().ToList();
            List<int> fromStorageLocations = sods.Select(a => (int)a.FromStorageLocation).Distinct().ToList();
            ret.stockOperationDetails = sods.OrderBy(a => a.OrderNo).ToList();
            // 获取移出库存货明细
            List<StockDetail> initSds = (await _stockOperationRepo.ListStockDetailByIDs(
                sods.Select(a => a.StockDetail).ToList()
                )).OrderBy(a => a.ID).ToList();
            // 获取原始数据的汇率、质保期、供应商等信息
            List<StockOperationDetail> initSodsInfo = (await _stockOperationRepo.ListSODByIDs(
                initSds.Select(a => a.StockOperationDetail).ToList())).ToList();
            //移库专用，移入，存货明细
            //ret.stockDetailsAdd = new List<StockDetail>();
            //ret.isAddStockDetails = false;
            ret.stockDetails = new List<StockDetail>();
            ret.stockDetailsUpdate = new List<StockDetail>();
            ret.stocks = new List<Stock>();
            ret.stockSums = new List<StockSum>();
            ret.wAlarmHistory = new List<WarehouseAlarmHistory>();
            List<int> sps = sods.Select(a => a.SpareParts).Distinct().ToList();
            List<WarehouseAlarm> was = await _warehouseAlarmRepo.GetBySPsAndWH(sps, so.Warehouse);
            List<WarehouseAlarm> wasTo = new List<WarehouseAlarm>();
            int toWarehouse = so.Warehouse;
            if (so.ToWarehouse != null)
            {
                toWarehouse = (int)so.ToWarehouse;
                wasTo = await _warehouseAlarmRepo.GetBySPsAndWH(sps, toWarehouse);
            }
            //应该是移出库库位转移到移入库库位，逻辑还要细想
            List<Stock> stocksTmp = await _stockOperationRepo.ListBySPsAndWH(sps.ToList(), so.Warehouse, fromStorageLocations);
            List<Stock> stocksTmpAll = await _stockOperationRepo.ListStockBySPs(sps);
            List<StockSum> stockSumsTmp = await _stockOperationRepo.ListBySPs(sps.ToList());

            // 存货明细数据整理
            foreach (StockOperationDetail sod in sods)
            {
                StockDetail sd = new StockDetail();
                //单件
                if (string.IsNullOrWhiteSpace(sod.NewEntity))
                {
                    //清空移出数量
                    sd = initSds.Where(a => a.ID == sod.StockDetail).FirstOrDefault();
                    sd.InStockNo = 0;
                    sd.StockNo = 0;
                    sd.TroubleNo = 0;
                    ret.stockDetailsUpdate.Add(sd);
                    //新增移入记录
                    StockDetail sdNew = GetBySod(sod, toWarehouse);
                    sdNew.StockOperationDetail = sd.StockOperationDetail;
                    ret.stockDetails.Add(sdNew);
                }
                //批次
                else
                {
                    //移出
                    sd = initSds.Where(a => a.ID == sod.StockDetail).FirstOrDefault();
                    switch (reason)
                    {
                        case StockOptDetailType.MoveTo:
                            sd.InStockNo = GetNo(StockChange.Subtract, sd.InStockNo, sod.CountNo);
                            if (sd.InStockNo < 0) { GetApiResult("存货不足，请刷新重试"); return ret; }
                            break;
                        case StockOptDetailType.TroubleMoveTo:
                            sd.TroubleNo = GetNo(StockChange.Subtract, sd.TroubleNo, sod.CountNo);
                            if (sd.TroubleNo < 0) { GetApiResult("故障件数量小于零，请刷新重试"); return ret; }
                            break;
                    }
                    sd.StockNo = GetNo(StockChange.Subtract, sd.StockNo, sod.CountNo);
                    if (sd.StockNo < 0) { GetApiResult("库存不足，请刷新重试"); return ret; }
                    ret.stockDetailsUpdate.Add(sd);
                    //移入
                    StockDetail sdNew = GetBySod(sod, toWarehouse);
                    sdNew.Entity = sod.NewEntity;
                    sdNew.StockOperationDetail = sd.StockOperationDetail;
                    ret.stockDetails.Add(sdNew);
                }
                StockOperationDetail initSodTmp = initSodsInfo.Where(a => a.ID == sd.StockOperationDetail).FirstOrDefault();
                sod.Currency = initSodTmp.Currency;
                sod.Supplier = initSodTmp.Supplier;
                sod.LifeDate = initSodTmp.LifeDate;
                sod.Invoice = initSodTmp.Invoice;
            }
            //移出库更新数据
            List<CompsiteKey> ckOld = sods.Select(a => new CompsiteKey
            {
                SpareParts = a.SpareParts,
                StorageLocation = (int)a.FromStorageLocation
            }).Distinct(new Comparer()).ToList();
            foreach (var item in ckOld)
            {
                // 仓库明细数据更新
                var sod = sods.Where(a => a.SpareParts == item.SpareParts && a.FromStorageLocation == item.StorageLocation);
                int countNo = sod.Sum(a => a.CountNo);
                //直接在前端换算成人民币
                double amount = sod.Sum(a => a.Amount);
                var sTmp = stocksTmp.Where(a => a.SpareParts == item.SpareParts && a.StockNo != 0
                && a.StorageLocation == item.StorageLocation);
                Stock s = sTmp.FirstOrDefault();
                s.IsAdd = false;
                switch(reason)
                {
                    case StockOptDetailType.TroubleMoveTo:
                    case StockOptDetailType.TroubleMoveLocation:
                        s.TroubleNo -= countNo;
                        break;
                    case StockOptDetailType.MoveTo:
                    case StockOptDetailType.MoveLocation:
                        s.InStockNo -= countNo;
                        break;
                }
                s.StockNo -= countNo;
                s.Amount -= amount;
                GetWAlarmHistories(so.Warehouse, item.SpareParts, was, ref s, ref ret);
                ret.stocks.Add(s);
            }
            //移入库插入数据
            List<CompsiteKey> ckNew = sods.Select(a => new CompsiteKey
            {
                SpareParts = a.SpareParts,
                StorageLocation = a.StorageLocation
            }).Distinct(new Comparer()).ToList();
            foreach (var item in ckNew)
            {
                // 仓库明细数据更新
                var sod = sods.Where(a => a.SpareParts == item.SpareParts && a.StorageLocation == item.StorageLocation);
                int countNo = sod.Sum(a => a.CountNo);
                //直接在前端换算成人民币
                double amount = sod.Sum(a => a.Amount);
                var sTmp = stocksTmp.Where(a => a.SpareParts == item.SpareParts && a.StockNo != 0
                && a.StorageLocation == item.StorageLocation);
                //int myWarehouse = (reason == StockOptDetailType.TroubleMoveTo
                //    || reason == StockOptDetailType.MoveTo) ? (int)so.ToWarehouse : so.Warehouse;
                Stock s =  GetStock(countNo, item.SpareParts, amount, toWarehouse, item.StorageLocation);
                s.IsAdd = true;
                switch (reason)
                {
                    case StockOptDetailType.TroubleMoveTo:
                    case StockOptDetailType.TroubleMoveLocation:
                        s.TroubleNo += countNo;
                        s.InStockNo -= countNo;
                        break;
                }
                if (wasTo.Count>0)
                {
                    GetWAlarmHistories(toWarehouse, item.SpareParts, wasTo, ref s, ref ret);
                }
                else
                {
                    GetWAlarmHistories(toWarehouse, item.SpareParts, was, ref s, ref ret);
                }
                ret.stocks.Add(s);
            }
            foreach (var item in sods.Select(a => a.SpareParts).Distinct())
            { 
                // 更新预警状态
                StockSum stockSum = stockSumsTmp.Where(a => a.SpareParts == item && a.StockNo != 0).FirstOrDefault();
                if (ret.stocks.Where(a => a.SpareParts == item && a.IsAlarm == 1).Count() > 0 
                    || stocksTmpAll.Where(a => a.SpareParts == item && a.IsAlarm == 1).Count() > 0)
                {
                    if (stockSum.IsAlarm == 0)
                    {
                        stockSum.IsAlarm = 1;
                        ret.stockSums.Add(stockSum);
                    }
                }
                else
                {
                    if (stockSum.IsAlarm == 1)
                    {
                        stockSum.IsAlarm = 0;
                        ret.stockSums.Add(stockSum);
                    }
                }
            }
            return ret;
        }


        private async Task<StockOperationSaveParm> GetParmInit(StockOperationSaveParm parm)
        {
            StockOperationSaveParm ret = new StockOperationSaveParm();
            StockOperation so = parm.stockOperation;
            ret.stockOperation = so;
            List<StockOperationDetail> sods = JsonConvert.DeserializeObject<List<StockOperationDetail>>(so.DetailList);
            List<int> storageLocations = sods.Select(a => a.StorageLocation).Distinct().ToList();
            if (await _stockOperationRepo.HasEntity(sods.Select(a => a.Entity).ToList()))
            {
                validateSaveData.code = Code.DataIsExist;
                validateSaveData.msg = "物资ID中与库存中已有的重复";
                return ret;
            }
            ret.stockOperationDetails = sods;

            ret.stockDetails = new List<StockDetail>();
            ret.stocks = new List<Stock>();
            ret.stockSums = new List<StockSum>();
            ret.wAlarmHistory = new List<WarehouseAlarmHistory>();

            List<int> sps = sods.Select(a => a.SpareParts).Distinct().ToList();
            List<WarehouseAlarm> was = await _warehouseAlarmRepo.GetBySPsAndWH(sps, so.Warehouse);
            List<Stock> stocksTmp = await _stockOperationRepo.ListBySPsAndWH(sps, so.Warehouse, storageLocations);
            List<Stock> stocksTmpAll = await _stockOperationRepo.ListStockBySPs(sps);
            List<StockSum> stockSumsTmp = await _stockOperationRepo.ListBySPs(sps.ToList());

            foreach (var item in sods)
            {
                StockDetail sd = GetBySod(item, so.Warehouse);
                ret.stockDetails.Add(sd);
            }

            foreach (var sp in sods.Select(a => a.SpareParts).Distinct())
            {
                var sod = sods.Where(a => a.SpareParts == sp);
                int countNo = sod.Sum(a => a.CountNo);
                double totalAmount = sod.Sum(a => a.TotalAmount);
                int isAlarm = 0;
                var safe = was.Where(a => a.SpareParts == sp);
                if (safe.Count() > 0)
                {
                    int safeValue = safe.FirstOrDefault().SafeStorage;
                    int stockNoNow = countNo;
                    if (stocksTmp != null || stocksTmp.Where(a => a.SpareParts == sp && a.StockNo != 0).Count() > 0)
                    {
                        stockNoNow += stocksTmp.Sum(a=>a.StockNo);
                    }
                    if (stockNoNow < safeValue)
                    {
                        isAlarm = 1;
                        ret.wAlarmHistory.Add(GetWAlarmHistory(so.Warehouse, sp, stockNoNow, safeValue));
                    }
                }
                foreach (var item in sods.Where(a=>a.SpareParts == sp).Select(a => a.StorageLocation).Distinct())
                {
                    var sodStock = sods.Where(a => a.SpareParts == sp && a.StorageLocation == item);
                    int countNoStock = sodStock.Sum(a => a.CountNo);
                    double totalAmountStock = sodStock.Sum(a => a.TotalAmount);
                    Stock s = new Stock();
                    var ssTmp = stocksTmp.Where(a => a.SpareParts == sp && a.StorageLocation == item && a.StockNo != 0);
                    if (ssTmp == null || ssTmp.Count() == 0)
                    {
                        s = GetStock(countNoStock, sp, totalAmountStock, so.Warehouse, item);
                    }
                    else
                    {
                        s = ssTmp.FirstOrDefault();
                        s.IsAdd = false;
                        s.InStockNo += countNoStock;
                        s.Amount += totalAmountStock;
                        s.StockNo += countNoStock;
                    }
                    s.IsAlarm = isAlarm;
                    ret.stocks.Add(s);
                }

                StockSum ss = new StockSum();
                if (stockSumsTmp == null || stockSumsTmp.Where(a => a.SpareParts == sp && a.StockNo != 0).Count() == 0)
                {
                    ss.IsAdd = true;
                    ss.InspectionNo = 0;
                    ss.InStockNo = countNo;
                    ss.RepairNo = 0;
                    ss.SpareParts = sp;
                    ss.Amount = totalAmount;
                    ss.StockNo = countNo;
                    ss.TroubleNo = 0;
                    ss.LentNo = 0;
                    ss.ScrapNo = 0;
                }
                else
                {
                    ss = stockSumsTmp.Where(a => a.SpareParts == sp && a.StockNo != 0).FirstOrDefault();
                    ss.IsAdd = false;
                    ss.InStockNo += countNo;
                    ss.Amount += totalAmount;
                    ss.StockNo += countNo;
                }
                ss.IsAlarm = isAlarm == 1 || stocksTmpAll.Where(a =>a.SpareParts==sp && a.IsAlarm == 1).Count() > 0 ? 1 : 0;
                ret.stockSums.Add(ss);
            }
            return ret;
        }
        /// <summary>
        /// 先只考虑单笔盘盈记录
        /// 默认盘盈记录必定曾经入库过
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        private async Task<StockOperationSaveParm> GetParmProfit(StockOperationSaveParm parm)
        {
            StockOperationSaveParm ret = new StockOperationSaveParm();
            StockOperation so = parm.stockOperation;
            ret.stockOperation = so;
            //在这里sod.id特指entity在stock_detail中对应的id
            StockOperationDetail sod = JsonConvert.DeserializeObject<StockOperationDetail>(so.DetailList);
            List<StockDetail> tmp = await _stockOperationRepo.GetStockDetailByEntitys(new List<string>() { sod.Entity }, so.Warehouse);
            if (tmp.Count == 0)
            {
                validateSaveData.code = Code.DataIsnotExist;
                validateSaveData.msg = "此仓库没有接收过此物资";
                return ret;
            }
            //if (await _stockOperationRepo.HasEntity(new List<string> { sod.Entity }))
            //{
            //    validateSaveData.code = Code.DataIsExist;
            //    validateSaveData.msg = "物资ID中与库存中已有的重复";
            //    return ret;
            //}
            StockDetail sd = tmp.FirstOrDefault();
            //sod.StockDetail = sd.ID;
            sod.SpareParts = sd.SpareParts;
            //新记录时将货币、供应商、保质期存入，方便按照原有逻辑查询
            StockOperationDetail initSodTmp = (await _stockOperationRepo.ListSODByIDs(new List<int> { sd.StockOperationDetail })).FirstOrDefault();
            sod.Currency = initSodTmp.Currency;
            sod.Supplier = initSodTmp.Supplier;
            sod.LifeDate = initSodTmp.LifeDate;
            sod.UnitPrice = initSodTmp.UnitPrice;
            sod.Amount = initSodTmp.Amount;
            sod.ExchangeRate = initSodTmp.ExchangeRate;
            ret.stockOperationDetails = new List<StockOperationDetail>() {sod };

            ret.stockDetails = new List<StockDetail>();
            ret.stocks = new List<Stock>();
            ret.stockSums = new List<StockSum>();
            ret.wAlarmHistory = new List<WarehouseAlarmHistory>();

            List<WarehouseAlarm> was = await _warehouseAlarmRepo.GetBySPsAndWH(new List<int>() { sd.SpareParts }, so.Warehouse);
            List<Stock> stocksTmp = await _stockOperationRepo.ListBySPsAndWH(new List<int>() {sd.SpareParts }, so.Warehouse,new List<int>() {sod.StorageLocation });
            List<Stock> stocksTmpAll = await _stockOperationRepo.ListStockBySPs(new List<int>() { sd.SpareParts });

            List<StockSum> stockSumsTmp = await _stockOperationRepo.ListBySPs(new List<int>() { sd.SpareParts });
            sd.InStockNo += sod.CountNo;
            sd.StockNo += sod.CountNo;
            sd.Status = (int)StockStatus.Profit;
            sd.Warehouse = sod.Warehouse;
            sd.StorageLocation = sod.StorageLocation;
            ret.stockDetails.Add(sd);

            double totalAmount = sod.CountNo * sd.AcceptUnitPrice * sd.ExchangeRate;
            Stock s = new Stock();
            if (stocksTmp.Count() == 0)
            {
                s = GetStock(sod.CountNo, sd.SpareParts, totalAmount, sod.Warehouse, sod.StorageLocation);
            }
            else
            {
                s = stocksTmp.FirstOrDefault();
                s.IsAdd = false;
                s.StockNo = s.StockNo + sod.CountNo;
                s.InStockNo = s.InStockNo + sod.CountNo;
                s.Amount = s.Amount + totalAmount;
            }
            GetWAlarmHistories(sod.Warehouse, sd.SpareParts, was, ref s, ref ret);
            ret.stocks.Add(s);

            stockSumsTmp[0].IsAdd = false;
            stockSumsTmp[0].StockNo = stockSumsTmp[0].StockNo + sod.CountNo;
            stockSumsTmp[0].InStockNo = stockSumsTmp[0].InStockNo + sod.CountNo;
            stockSumsTmp[0].Amount = stockSumsTmp[0].Amount + totalAmount;
            stockSumsTmp[0].IsAlarm= s.IsAlarm == 1 || stocksTmpAll.Where(a => a.IsAlarm == 1).Count() > 0?1:0;
            ret.stockSums.Add(stockSumsTmp[0]);
            return ret;
        }

        #region 非主要的私有函数
        class CompsiteKey
        {
            public int SpareParts { get; set; }
            public int StorageLocation { get; set; }
        }

        class Comparer : IEqualityComparer<CompsiteKey>
        {
            public bool Equals(CompsiteKey x, CompsiteKey y)
            {
                if (x == null && y == null)
                {
                    return false;
                }
                else
                {
                    return x.SpareParts == y.SpareParts && x.StorageLocation == y.StorageLocation;
                }
            }

            public int GetHashCode(CompsiteKey obj)
            {
                return obj.ToString().GetHashCode();
            }
        }
        /// <summary>
        /// 入库新库位
        /// </summary>
        /// <param name="sod"></param>
        /// <param name="warehouse"></param>
        /// <returns></returns>
        private StockDetail GetBySod(StockOperationDetail sod, int warehouse)
        {
            StockDetail sd = new StockDetail();
            sd.Entity = sod.Entity;
            sd.InspectionNo = 0;
            sd.InStockNo = sod.CountNo;
            sd.RepairNo = 0;
            sd.SpareParts = sod.SpareParts;
            sd.Status = sod.CountNo > 1 ? (int)StockStatus.None : (int)StockStatus.Normal;
            sd.StockNo = sod.CountNo;
            sd.TroubleNo = 0;
            sd.LentNo = 0;
            sd.ScrapNo = 0;
            sd.Warehouse = warehouse;
            //sd.StockOperationDetail = sod.ID;
            sd.IsBatch = sod.CountNo > 1 ? 1 : 0;
            sd.StorageLocation = sod.StorageLocation;
            return sd;
        }
        /// <summary>
        /// 将原StockDetail的借用/送修/送检归还数量减去
        /// 新库位的对应数量加上
        /// </summary>
        /// <param name="sdt"></param>
        /// <param name="countNo"></param>
        /// <param name="sd"></param>
        /// <param name="oldSd"></param>
        private void GetStockDetailByReason(StockOptDetailType sdt, int countNo,
            ref StockDetail sd, ref StockDetail oldSd)
        {
            switch (sdt)
            {
                case StockOptDetailType.TroubleReturn:
                    sd.TroubleNo += countNo;
                    sd.InStockNo = 0;
                    sd.Status = sd.StockNo > 1 ? (int)StockStatus.None : (int)StockStatus.Trouble;
                    oldSd = null;
                    break;
                case StockOptDetailType.lendReturn:
                    oldSd.StockNo -= countNo;
                    oldSd.LentNo -= countNo;
                    if (oldSd.LentNo < 0) { GetApiResult("外借数量小于零，请刷新重试"); }
                    break;
                case StockOptDetailType.RepairReceive:
                    oldSd.StockNo -= countNo;
                    oldSd.RepairNo -= countNo;
                    if (oldSd.RepairNo < 0) { GetApiResult("送修数量小于零，请刷新重试"); }
                    break;
                case StockOptDetailType.InspectionReturn:
                    oldSd.StockNo -= countNo;
                    oldSd.InspectionNo -= countNo;
                    if (oldSd.InspectionNo < 0) { GetApiResult("送检数量小于零，请刷新重试"); }
                    break;
                default:
                    oldSd = null;
                    break;
            }
        }
        /// <summary>
        /// 新仓库记录Stock
        /// </summary>
        /// <param name="countNo"></param>
        /// <param name="sp"></param>
        /// <param name="amount"></param>
        /// <param name="warehouse"></param>
        /// <param name="storageLocation"></param>
        /// <returns></returns>
        private Stock GetStock(int countNo, int sp, double amount, int warehouse, int storageLocation)
        {
            Stock s = new Stock();
            s.IsAdd = true;
            s.InspectionNo = 0;
            s.InStockNo = countNo;
            s.RepairNo = 0;
            s.SpareParts = sp;
            s.Amount = amount;
            s.StockNo = countNo;
            s.TroubleNo = 0;
            s.LentNo = 0;
            s.ScrapNo = 0;
            s.Warehouse = warehouse;
            s.StorageLocation = storageLocation;
            //s.IsAlarm = 0;
            return s;
        }

        private void GetStockUpdate(StockOperationDetail sod, StockChange sc, int warehouse,
            StockOptDetailType reason, List<Stock> stocksTmp, ref Stock s, ref StockOperationSaveParm ret)
        {
            int StorageLocation;
            if (sc == StockChange.Plus) StorageLocation = sod.StorageLocation;
            else StorageLocation = (int)sod.FromStorageLocation;
            var sTmp = ret.stocks.Where(a => a.SpareParts == sod.SpareParts && a.StockNo != 0
                    && a.Warehouse == warehouse && a.StorageLocation == StorageLocation);
            if (sTmp == null || sTmp.Count() == 0)
            {
                var newS = stocksTmp.Where(a => a.SpareParts == sod.SpareParts && a.StockNo != 0
                  && a.Warehouse == warehouse && a.StorageLocation == StorageLocation);
                if (newS == null || newS.Count() == 0)
                {
                    s = GetStock(sod.CountNo, sod.SpareParts, sod.Amount, warehouse, sod.StorageLocation);
                }
                else
                {
                    s = newS.FirstOrDefault();
                    GetNoByReason(reason, sc, sod.CountNo, sod.Amount, ref s);
                }
                ret.stocks.Add(s);
            }
            else
            {
                s = sTmp.FirstOrDefault();
                GetNoByReason(reason, sc, sod.CountNo, sod.Amount, ref s);
            }
        }

        private void GetNoByReason(StockOptDetailType reason, StockChange sc, int count, double amount, ref Stock s)
        {
            s.IsAdd = false;
            switch (reason)
            {
                case StockOptDetailType.lendReturn:
                    if (sc == StockChange.Plus)
                    {
                        s.StockNo += count;
                        s.InStockNo += count;
                        s.Amount += amount;
                    }
                    else
                    {
                        s.LentNo -= count;
                        s.StockNo -= count;
                        s.Amount -= amount;
                    }
                    break;
                case StockOptDetailType.RepairReceive:
                    if (sc == StockChange.Plus)
                    {
                        s.StockNo += count;
                        s.InStockNo += count;
                        s.Amount += amount;
                    }
                    else
                    {
                        s.RepairNo -= count;
                        s.StockNo -= count;
                        s.Amount -= amount;
                    }
                    break;
                case StockOptDetailType.InspectionReturn:
                    if (sc == StockChange.Plus)
                    {
                        s.StockNo += count;
                        s.InStockNo += count;
                        s.Amount += amount;
                    }
                    else
                    {
                        s.InspectionNo -= count;
                        s.StockNo -= count;
                        s.Amount -= amount;
                    }
                    break;
                    //case StockOptDetailType.Inspection:
                    //    s.InStockNo -= count;
                    //    s.InspectionNo += count;
                    //    break;
                    //case StockOptDetailType.MaterialLend:
                    //    s.InStockNo -= count;
                    //    s.LentNo += count;
                    //    break;
                    //case StockOptDetailType.TroubleRepair:
                    //    s.InStockNo -= count;
                    //    s.RepairNo += count;
                    //    break;
                    //default:
                    //    s.InStockNo += count;
                    //    s.StockNo += count;
                    //    break;
            }
        }
        /// <summary>
        /// 出入库位不同时的通用函数
        /// 入库存货增加、新库位时，存货和金额增加、修改原库位、原库位库存减少
        /// </summary>
        /// <param name="countNo"></param>
        /// <param name="isNew"></param>
        /// <param name="sd"></param>
        /// <param name="oldSd"></param>

        private int GetNo(StockChange sc, int initNo, int editNo)
        {
            switch (sc)
            {
                case StockChange.NoChange: return initNo;
                case StockChange.Plus: return initNo + editNo;
                case StockChange.Subtract: return initNo - editNo;
                default: return initNo;
            }
        }

        private double GetAmount(StockChange sc, double initNo, double editNo)
        {
            switch (sc)
            {
                case StockChange.NoChange: return initNo;
                case StockChange.Plus: return initNo + editNo;
                case StockChange.Subtract: return initNo - editNo;
                default: return initNo;
            }
        }

        /// <summary>
        /// 库存操作保存专用
        /// </summary>
        /// <param name="msg"></param>
        private void GetApiResult(string msg)
        {
            validateSaveData.code = Code.CheckDataRulesFail;
            validateSaveData.msg = msg;
        }
        private void GetWAlarmHistories(int warehouse,int spareParts,
            List<WarehouseAlarm> was,ref Stock stock,ref StockOperationSaveParm ret)
            
        {
            stock.IsAlarm = 0;
            var walarm = was.Where(a => a.SpareParts == spareParts && a.Warehouse==warehouse);
            if (walarm.Count() > 0)
            {
                int safeValue = walarm.FirstOrDefault().SafeStorage;
                if (stock.StockNo < safeValue)
                {
                    stock.IsAlarm = 1;
                    ret.wAlarmHistory.Add(GetWAlarmHistory(warehouse,spareParts,stock.StockNo,safeValue));
                }
            }
        }

        private WarehouseAlarmHistory GetWAlarmHistory(int warehouse, int spareParts,int stockNo,int safeValue)
        {
            WarehouseAlarmHistory wah = new WarehouseAlarmHistory();
            wah.StockNo = stockNo;
            wah.SafeStorage = safeValue;
            wah.SpareParts = spareParts;
            wah.Warehouse = warehouse;
            wah.CreatedTime = DateTime.Now;
            return wah;
        }
        #endregion

        #region 采购退货用到的采购接收流水号下拉、下拉选中后对应的记录显示
        public async Task<ApiResult> ListByReason(int reason)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _stockOperationRepo.ListByReason(reason);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> ListByOperation(int operation)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _stockOperationRepo.ListStockDetailByOperation(operation);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }
        #endregion

        /// <summary>
        /// 根据仓库找物资（级联筛选）
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public async Task<ApiResult> ListByWH(int warehouse)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _stockOperationRepo.ListByWH(warehouse);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        /// <summary>
        /// 直接从存货中所有物资ID
        /// </summary>
        /// <returns></returns>
        public async Task<ApiResult> ListStockDetail(int reason)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _stockOperationRepo.ListStockDetail((StockOptDetailType)reason);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        #endregion

        #region 库存操作查询
        public async Task<ApiResult> GetPageByParm(StockOperationQueryParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows = parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                ret.data = await _stockOperationRepo.GetPageByParm(parm);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isEdit">编辑时需要排除一些数据，查看时一般查看所有</param>
        /// <returns></returns>
        public async Task<ApiResult> GetByID(int id,bool isEdit=false)
        {
            ApiResult ret = new ApiResult();
            try
            {
                StockOperation s = await _stockOperationRepo.GetByID(id);
                StockOptDetailType reason = (StockOptDetailType)s.Reason;
                int fromID = 0;
                switch (reason)
                {
                    case StockOptDetailType.PurchaseReceive:
                    case StockOptDetailType.OtherReceive:
                        //s.DetailList = JsonConvert.SerializeObject(await _stockOperationRepo.ListByOperationIn(id));
                        //break;
                    case StockOptDetailType.InventoryProfit:
                        //case StockOptDetailType.InStockScrap:
                        s.DetailList = JsonConvert.SerializeObject(await _stockOperationRepo.ListByOperationIn(id,true));
                        break;
                    //case StockOptDetailType.Distribution:
                    //case StockOptDetailType.Inspection:
                    //case StockOptDetailType.MaterialLend:
                    //case StockOptDetailType.TroubleRepair:
                    //    s.DetailList = JsonConvert.SerializeObject(await _stockOperationRepo.ListByOperationOut(id));
                    //    break;
                    default:
                        List<StockOperationDetail> tmp = await _stockOperationRepo.ListByOperationOut(id,isEdit);
                        StockOperationDetail sod = tmp.FirstOrDefault();
                        fromID = sod.FromStockOperationDetail;
                        s.SomeOrder = sod.SomeOrder;
                        s.WorkingOrder = sod.WorkingOrder;
                        s.DetailList = JsonConvert.SerializeObject(tmp);
                        break;
                }
                if (reason == StockOptDetailType.TroubleReturn || reason==StockOptDetailType.lendReturn ||
                    reason==StockOptDetailType.MaterialReturn || reason==StockOptDetailType.RepairReceive ||
                    reason==StockOptDetailType.InspectionReturn)
                {
                    List<StockOperationDetail> tmp = await _stockOperationRepo.ListSODByIDs(new List<int> { fromID });
                    StockOperationDetail sod = tmp.FirstOrDefault();
                    s.SomeOrder = sod.SomeOrder;
                    s.WorkingOrder = sod.WorkingOrder;
                }
                ret.data = s;
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }
        #endregion

        #region 库存查询
        public async Task<ApiResult> GetStockSumPageByParm(StockSumQueryParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows = parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                if (parm.SearchWarehouse == null)
                {
                    StockSumView ssv = await _stockOperationRepo.GetStockSumPageByParm(parm);
                    List<Stock> stocks;
                    stocks = await _stockOperationRepo.ListStockBySPs(ssv.rows.Select(a => a.SpareParts).Distinct().ToList());
                    foreach (var item in ssv.rows)
                    {
                        var tmp = stocks.Where(a => a.SpareParts == item.SpareParts);
                        item.stocks = tmp == null ? null : tmp.ToList();
                    }
                    ret.data = ssv;
                }
                else
                {
                    ret.data = await _stockOperationRepo.GetStockPageByParm(parm);
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

        #endregion

        #region 存货明细
        public async Task<ApiResult> ListStockDetailBySPsAndWH(int spareParts,int warehouse, 
            StockOptDetailType reason,int storageLocation)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _stockOperationRepo
                    .ListStockDetailBySPsAndWH(new List<int> { spareParts },warehouse,false,reason,storageLocation);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }


        public async Task<ApiResult> GetStockDetailByID(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _stockOperationRepo.GetStockDetailByID(id);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        #endregion
    }
}
