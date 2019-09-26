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
        private readonly IDistributedCache _cache;

        private readonly int userID;
        private ApiResult validateSaveData=new ApiResult();
        public StockOperationService(IStockOperationRepo<StockOperation> stockOperationRepo, 
            IAuthHelper auth, IDistributedCache cache)
        {
            //_logger = logger;
            _stockOperationRepo = stockOperationRepo;
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
                                sosp = await GetParm(
                                    StockChange.NoChange, StockChange.NoChange, StockChange.Plus,StockChange.NoChange,
                                    StockChange.NoChange, StockChange.Subtract, StockChange.NoChange, StockStatus.Normal, 
                                    parm, StockOperationStatus.NoReturn);
                                if (validateSaveData.code != Code.Success) return validateSaveData;
                                intTmp = await _stockOperationRepo.Save(sosp);
                                break;
                            case StockOptDetailType.RepairReceive:
                                sosp = await GetParm(
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
                                sosp = await GetParm(
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

        private async Task<StockOperationSaveParm> GetParm(StockChange stock, StockChange trouble, StockChange inStock,StockChange inspection,
            StockChange repair, StockChange lent, StockChange scrap, StockStatus status,StockOperationSaveParm parm, 
            StockOperationStatus soStatus= StockOperationStatus.Other)
        {
            StockOperationSaveParm ret = new StockOperationSaveParm();
            StockOperation so = parm.stockOperation;
            ret.stockOperation = so;
            List<StockOperationDetail> sods = JsonConvert.DeserializeObject<List<StockOperationDetail>>(so.DetailList).OrderBy(a => a.StockDetail).ToList();
            ret.stockOperationDetails = sods.OrderBy(a=>a.OrderNo).ToList();
            // 获取原始出库数量以及已归(退)还数量
            List<StockOperationDetail> initSods = (await _stockOperationRepo.ListSODByIDs(
                sods.Select(a => a.FromStockOperationDetail).ToList())).OrderBy(a=>a.StockDetail).ToList();
            // 获取存货明细
            List<StockDetail> initSds = (await _stockOperationRepo.ListStockDetailByIDs(
                sods.Select(a => a.StockDetail).ToList()
                )).OrderBy(a=>a.ID).ToList();
            ret.isAddStockDetails = false;
            ret.stockDetails = new List<StockDetail>();
            ret.stocks = new List<Stock>();
            ret.stockSums = new List<StockSum>();
            List<Stock> stocksTmp = await _stockOperationRepo.ListBySPsAndWH(sods.Select(a => a.SpareParts).Distinct().ToList(), so.Warehouse);
            List<StockSum> stockSumsTmp = await _stockOperationRepo.ListBySPs(sods.Select(a => a.SpareParts).Distinct().ToList());
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
            // 存货明细数据更新
            for (int i = 0; i < initSds.Count; i++)
            {
                StockDetail sd = new StockDetail();
                sd.ID = initSds[i].ID;
                sd.StockNo = GetNo(stock, initSds[i].StockNo, sods[i].CountNo);
                if (sd.StockNo < 0) { GetApiResult("库存不足，请刷新重试"); return ret; }
                sd.TroubleNo = GetNo(trouble, initSds[i].TroubleNo, sods[i].CountNo);
                if (sd.TroubleNo < 0) { GetApiResult("故障件数量小于零，请刷新重试"); return ret; }
                sd.InStockNo = GetNo(inStock, initSds[i].InStockNo, sods[i].CountNo);
                if (sd.InStockNo < 0) { GetApiResult("存货不足，请刷新重试"); return ret; } 
                sd.InspectionNo = GetNo(inspection, initSds[i].InspectionNo, sods[i].CountNo);
                if (sd.InspectionNo < 0) {GetApiResult("送检数量小于零，请刷新重试"); return ret; }
                sd.RepairNo = GetNo(repair, initSds[i].RepairNo, sods[i].CountNo);
                if (sd.RepairNo < 0) {GetApiResult("送修数量小于零，请刷新重试"); return ret; }
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
                ret.stockDetails.Add(sd);
            }
            foreach (var item in sods.Select(a => a.SpareParts).Distinct())
            {
                // 仓库明细数据更新
                var sod = sods.Where(a => a.SpareParts == item);
                int countNo = sod.Sum(a => a.CountNo);
                //直接在前端换算成人民币
                double amount = sod.Sum(a => a.Amount);
                Stock s = stocksTmp.Where(a => a.SpareParts == item).FirstOrDefault();
                s.isAdd = false;
                s.StockNo = GetNo(stock, s.StockNo, countNo);
                s.TroubleNo = GetNo(trouble, s.TroubleNo, countNo);
                s.InStockNo = GetNo(inStock, s.InStockNo, countNo);
                s.InspectionNo = GetNo(inspection, s.InspectionNo, countNo);
                s.RepairNo = GetNo(repair, s.RepairNo, countNo);
                s.LentNo = GetNo(lent, s.LentNo, countNo);
                s.ScrapNo = GetNo(scrap, s.ScrapNo, countNo);
                s.Amount = GetAmount(stock, s.Amount, amount);
                ret.stocks.Add(s);
                // 库存总表数据更新
                StockSum ss = stockSumsTmp.Where(a => a.SpareParts == item).FirstOrDefault();
                ss.isAdd = false;
                ss.StockNo = GetNo(stock, ss.StockNo, countNo);
                ss.TroubleNo = GetNo(trouble, ss.TroubleNo, countNo);
                ss.InStockNo = GetNo(inStock, ss.InStockNo, countNo);
                ss.InspectionNo = GetNo(inspection, ss.InspectionNo, countNo);
                ss.RepairNo = GetNo(repair, ss.RepairNo, countNo);
                ss.LentNo = GetNo(lent, ss.LentNo, countNo);
                ss.ScrapNo = GetNo(scrap, ss.ScrapNo, countNo);
                ss.Amount = GetAmount(stock, ss.Amount, amount);
                ret.stockSums.Add(ss);
            }
            return ret;
        }

        private int GetNo(StockChange sc,int initNo,int editNo)
        {
            switch (sc)
            {
                case StockChange.NoChange:return initNo;
                case StockChange.Plus: return initNo+editNo;
                case StockChange.Subtract: return initNo-editNo;
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

        //移库
        //sum总表不变
        //存货批次移库时，类似领料(更新)->再入库(插入)的操作，需要新的物资ID
        //存货单件时，只要更新仓库即可
        //仓库库存表stock中，类似领料再入库的操作，只和sparePartsID和warehouseID有关
        private async Task<StockOperationSaveParm> GetParmMove(StockOperationSaveParm parm)
        {
            StockOperationSaveParm ret = new StockOperationSaveParm();
            StockOperation so = parm.stockOperation;
            ret.stockOperation = so;
            List<StockOperationDetail> sods = JsonConvert.DeserializeObject<List<StockOperationDetail>>(so.DetailList).OrderBy(a => a.StockDetail).ToList();
            ret.stockOperationDetails = sods.OrderBy(a => a.OrderNo).ToList();
            // 获取移出库存货明细
            List<StockDetail> initSds = (await _stockOperationRepo.ListStockDetailByIDs(
                sods.Select(a => a.StockDetail).ToList()
                )).OrderBy(a => a.ID).ToList();
            //移库专用，移入，存货明细
            ret.stockDetailsAdd = new List<StockDetail>();
            ret.isAddStockDetails = false;
            ret.stockDetails = new List<StockDetail>();
            ret.stocks = new List<Stock>();
            ret.stockSums = new List<StockSum>();
            List<Stock> stocksTmp = await _stockOperationRepo.ListBySPsAndWH(sods.Select(a => a.SpareParts).Distinct().ToList(), so.Warehouse);
            List<Stock> stocksTmpTo = await _stockOperationRepo.ListBySPsAndWH(sods.Select(a => a.SpareParts).Distinct().ToList(), (int)so.ToWarehouse);

            if (await _stockOperationRepo.HasEntity(sods.Where(a=>!string.IsNullOrWhiteSpace(a.NewEntity)).Select(a => a.NewEntity).ToList()))
            {
                validateSaveData.code = Code.DataIsExist;
                validateSaveData.msg = "物资ID中与库存中已有的重复";
                return ret;
            }

            // 存货明细数据整理
            foreach (StockOperationDetail sod in sods)
            {
                //单件
                if (string.IsNullOrWhiteSpace(sod.NewEntity))
                {
                    StockDetail sd = initSds.Where(a => a.ID == sod.StockDetail).FirstOrDefault();
                    sd.Warehouse = (int)so.ToWarehouse;
                    ret.stockDetails.Add(sd);
                }
                //批次
                else
                {
                    //移出
                    StockDetail sd = initSds.Where(a => a.ID == sod.StockDetail).FirstOrDefault();
                    if (so.Reason == (int)StockOptDetailType.MoveTo)
                    {
                        sd.InStockNo = GetNo(StockChange.Subtract, sd.InStockNo, sod.CountNo);
                        if (sd.InStockNo < 0) { GetApiResult("存货不足，请刷新重试"); return ret; }
                    }
                    else if (so.Reason == (int)StockOptDetailType.TroubleMoveTo)
                    {
                        sd.TroubleNo = GetNo(StockChange.Subtract, sd.TroubleNo, sod.CountNo);
                        if (sd.TroubleNo < 0) { GetApiResult("故障件数量小于零，请刷新重试"); return ret; }
                    }
                    sd.StockNo = GetNo(StockChange.Subtract, sd.StockNo, sod.CountNo);
                    if (sd.StockNo < 0) { GetApiResult("库存不足，请刷新重试"); return ret; }
                    ret.stockDetails.Add(sd);
                    //移入
                    StockDetail sdNew = new StockDetail();
                    sdNew.Entity = sod.NewEntity;
                    sdNew.InspectionNo = 0;
                    sdNew.InStockNo = sod.CountNo;
                    sdNew.RepairNo = 0;
                    sdNew.SpareParts = sd.SpareParts;
                    sdNew.Status = (int)StockStatus.None;
                    sdNew.StockNo = sod.CountNo;
                    sdNew.TroubleNo = 0;
                    sdNew.LentNo = 0;
                    sdNew.ScrapNo = 0;
                    sdNew.Warehouse = (int)so.ToWarehouse;
                    sdNew.StockOperationDetail = sd.StockOperationDetail;
                    ret.stockDetailsAdd.Add(sdNew);
                }
            }

            foreach (var item in sods.Select(a => a.SpareParts).Distinct())
            {
                // 仓库明细数据更新
                var sod = sods.Where(a => a.SpareParts == item);
                int countNo = sod.Sum(a => a.CountNo);
                //直接在前端换算成人民币
                double amount = sod.Sum(a => a.Amount);
                // 移出
                Stock s = stocksTmp.Where(a => a.SpareParts == item).FirstOrDefault();
                s.isAdd = false;
                if (so.Reason == (int)StockOptDetailType.MoveTo)
                {
                    s.StockNo = GetNo(StockChange.Subtract, s.StockNo, countNo);
                }
                else
                {
                    s.TroubleNo = GetNo(StockChange.Subtract, s.TroubleNo, countNo);
                }
                s.InStockNo = GetNo(StockChange.Subtract, s.InStockNo, countNo);
                s.Amount= GetAmount(StockChange.Subtract, s.Amount, amount);
                ret.stocks.Add(s);
                // 存在移入(更新)
                var tmpTo = stocksTmpTo.Where(a => a.SpareParts == item);
                if (tmpTo.Count() > 0)
                {
                    Stock sTo = tmpTo.FirstOrDefault();
                    sTo.isAdd = false;
                    if (so.Reason == (int)StockOptDetailType.MoveTo)
                    {
                        sTo.InStockNo = GetNo(StockChange.Plus, sTo.InStockNo, countNo);
                    }
                    else
                    {
                        sTo.TroubleNo = GetNo(StockChange.Plus, sTo.TroubleNo, countNo);
                    }
                    sTo.StockNo = GetNo(StockChange.Plus, sTo.StockNo, countNo);
                    sTo.Amount = GetAmount(StockChange.Plus, sTo.Amount, amount);
                    ret.stocks.Add(sTo);
                }
                // 不存在移入(插入)
                else
                {
                    Stock sTo = new Stock();
                    sTo.isAdd = true;
                    if (so.Reason == (int)StockOptDetailType.MoveTo)
                    {
                        sTo.InStockNo = countNo;
                    }
                    else
                    {
                        sTo.TroubleNo = countNo;
                    }
                    sTo.StockNo = countNo;
                    sTo.SpareParts = item;
                    sTo.Amount = amount;
                    sTo.Warehouse = (int)so.ToWarehouse;
                    ret.stocks.Add(sTo);
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
            if (await _stockOperationRepo.HasEntity(sods.Select(a => a.Entity).ToList()))
            {
                validateSaveData.code = Code.DataIsExist;
                validateSaveData.msg = "物资ID中与库存中已有的重复";
                return ret;
            }
            ret.stockOperationDetails = sods;

            ret.isAddStockDetails = true;
            ret.stockDetails = new List<StockDetail>();
            ret.stocks = new List<Stock>();
            ret.stockSums = new List<StockSum>();

            List<Stock> stocksTmp = await _stockOperationRepo.ListBySPsAndWH(sods.Select(a => a.SpareParts).Distinct().ToList(), so.Warehouse);

            List<StockSum> stockSumsTmp = await _stockOperationRepo.ListBySPs(sods.Select(a => a.SpareParts).Distinct().ToList());
            foreach (var item in sods)
            {
                StockDetail sd = new StockDetail();
                sd.Entity = item.Entity;
                sd.InspectionNo = 0;
                sd.InStockNo = item.CountNo;
                sd.RepairNo = 0;
                sd.SpareParts = item.SpareParts;
                sd.Status = item.CountNo>1? (int)StockStatus.None:(int)StockStatus.Normal;
                sd.StockNo = item.CountNo;
                //sd.StockOperationDetail = item.ID;此ID需要在DAL层事务中赋值
                sd.TroubleNo = 0;
                sd.LentNo = 0;
                sd.ScrapNo = 0;
                sd.Warehouse = so.Warehouse;
                ret.stockDetails.Add(sd);
            }

            foreach (var item in sods.Select(a => a.SpareParts).Distinct())
            {
                var sod = sods.Where(a => a.SpareParts == item);
                int countNo = sod.Sum(a => a.CountNo);
                double totalAmount = sod.Sum(a => a.TotalAmount);
                if (stocksTmp == null || stocksTmp.Where(a=>a.SpareParts==item).Count() == 0)
                {
                    Stock s = new Stock();
                    s.isAdd = true;
                    s.InspectionNo = 0;
                    s.InStockNo = countNo;
                    s.RepairNo = 0;
                    s.SpareParts = item;
                    s.Amount = totalAmount;
                    s.StockNo = countNo;
                    s.TroubleNo = 0;
                    s.LentNo = 0;
                    s.ScrapNo = 0;
                    s.Warehouse = so.Warehouse;
                    ret.stocks.Add(s);
                }
                else
                {
                    Stock s = stocksTmp.Where(a=>a.SpareParts==item).FirstOrDefault();
                    s.isAdd = false;
                    s.InStockNo += countNo;
                    s.Amount += totalAmount;
                    s.StockNo += countNo;
                    ret.stocks.Add(s);
                }

                if (stockSumsTmp == null || stockSumsTmp.Where(a => a.SpareParts == item).Count() == 0)
                {
                    StockSum ss = new StockSum();
                    ss.isAdd = true;
                    ss.InspectionNo = 0;
                    ss.InStockNo = countNo;
                    ss.RepairNo = 0;
                    ss.SpareParts = item;
                    ss.Amount = totalAmount;
                    ss.StockNo = countNo;
                    ss.TroubleNo = 0;
                    ss.LentNo = 0;
                    ss.ScrapNo = 0;
                    ret.stockSums.Add(ss);
                }
                else
                {
                    StockSum ss = stockSumsTmp.Where(a => a.SpareParts == item).FirstOrDefault();
                    ss.isAdd = false;
                    ss.InStockNo += countNo;
                    ss.Amount += totalAmount;
                    ss.StockNo += countNo;
                    ret.stockSums.Add(ss);
                }
            }
            return ret;
        }
        /// <summary>
        /// 先只考虑单笔盘盈记录
        /// </summary>
        /// <param name="parm"></param>
        /// <returns></returns>
        private async Task<StockOperationSaveParm> GetParmProfit(StockOperationSaveParm parm)
        {
            StockOperationSaveParm ret = new StockOperationSaveParm();
            StockOperation so = parm.stockOperation;
            ret.stockOperation = so;
            StockOperationDetail sod = JsonConvert.DeserializeObject<StockOperationDetail>(so.DetailList);
            List<StockDetail> tmp = await _stockOperationRepo.GetStockDetailByEntitys(new List<string>() {sod.Entity });
            if (tmp.Count==0)
            {
                validateSaveData.code = Code.DataIsnotExist;
                validateSaveData.msg = "仓库没有接收过此物资";
                return ret;
            }
            StockDetail sd = tmp.FirstOrDefault();
            sod.StockDetail = sd.ID;
            sod.SpareParts = sd.SpareParts;
            ret.stockOperationDetails = new List<StockOperationDetail>() {sod };

            ret.isAddStockDetails = false;
            ret.stockDetails = new List<StockDetail>();
            ret.stocks = new List<Stock>();
            ret.stockSums = new List<StockSum>();

            List<Stock> stocksTmp = await _stockOperationRepo.ListBySPsAndWH(new List<int>() {sd.SpareParts }, so.Warehouse);

            List<StockSum> stockSumsTmp = await _stockOperationRepo.ListBySPs(new List<int>() { sd.SpareParts });
            sd.InStockNo = sd.InStockNo + sod.CountNo;
            sd.StockNo = sd.StockNo + sod.CountNo;
            sd.Status = sd.StockNo > 1 ? (int)StockStatus.None : (int)StockStatus.Profit;
            sd.Warehouse = sod.Warehouse;
            ret.stockDetails.Add(sd);

            double totalAmount = sod.CountNo * sd.AcceptUnitPrice * sd.ExchangeRate;
            stocksTmp[0].isAdd = false;
            stocksTmp[0].StockNo = stocksTmp[0].StockNo + sod.CountNo;
            stocksTmp[0].InStockNo = stocksTmp[0].InStockNo + sod.CountNo;
            stocksTmp[0].Amount = stocksTmp[0].Amount + totalAmount;
            ret.stocks.Add(stocksTmp[0]);

            stockSumsTmp[0].isAdd = false;
            stockSumsTmp[0].StockNo = stockSumsTmp[0].StockNo + sod.CountNo;
            stockSumsTmp[0].InStockNo = stockSumsTmp[0].InStockNo + sod.CountNo;
            stockSumsTmp[0].Amount = stockSumsTmp[0].Amount + totalAmount;
            ret.stockSums.Add(stockSumsTmp[0]);
            return ret;
        }

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
        public async Task<ApiResult> ListStockDetail()
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _stockOperationRepo.ListStockDetail();
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

        public async Task<ApiResult> GetByID(int id)
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
                    //case StockOptDetailType.TroubleScrap:
                    //case StockOptDetailType.InStockScrap:
                        s.DetailList = JsonConvert.SerializeObject(await _stockOperationRepo.ListByOperationIn(id));
                        break;
                    //case StockOptDetailType.Distribution:
                    //case StockOptDetailType.Inspection:
                    //case StockOptDetailType.MaterialLend:
                    //case StockOptDetailType.TroubleRepair:
                    //    s.DetailList = JsonConvert.SerializeObject(await _stockOperationRepo.ListByOperationOut(id));
                    //    break;
                    default:
                        List<StockOperationDetail> tmp = await _stockOperationRepo.ListByOperationOut(id);
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
        public async Task<ApiResult> ListStockDetailBySPsAndWH(int spareParts,int warehouse)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _stockOperationRepo.ListStockDetailBySPsAndWH(new List<int> { spareParts },warehouse);
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
