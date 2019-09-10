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
                //构造存货明细表、库存明细表、库存总表
                switch (t)
                {
                    case StockOperationType.Receive:
                        switch ((StockOptDetailType)stockOperation.Reason)
                        {
                            case StockOptDetailType.PurchaseReceive:
                            case StockOptDetailType.OtherReceive:
                                intTmp = await _stockOperationRepo.Save(await GetParmInit(parm));
                                break;
                            case StockOptDetailType.PurchaseReturn:
                                StockOperationSaveParm sosp = await GetParm(
                                    StockChange.Subtract, StockChange.NoChange, StockChange.Subtract,
                                    StockChange.NoChange, StockChange.NoChange, StockStatus.Normal, parm);
                                if (validateSaveData.code != Code.Success) return validateSaveData;
                                intTmp = await _stockOperationRepo.Save(sosp);
                                break;
                        }
                        break;
                    case StockOperationType.Delivery:
                        break;
                    case StockOperationType.Adjust:
                        break;
                    case StockOperationType.Move:
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
            StockChange repair, StockStatus status,StockOperationSaveParm parm)
        {
            StockOperationSaveParm ret = new StockOperationSaveParm();
            StockOperation so = parm.stockOperation;
            ret.stockOperation = so;
            List<StockOperationDetail> sods = JsonConvert.DeserializeObject<List<StockOperationDetail>>(so.DetailList);
            ret.stockOperationDetails = sods;

            List<StockDetail> initSds = (await _stockOperationRepo.ListStockDetailByIDs(
                sods.Select(a => a.StockDetail).ToList()
                )).OrderBy(a=>a.ID).ToList();
            ret.isAddStockDetails = false;
            ret.stockDetails = new List<StockDetail>();
            ret.stocks = new List<Stock>();
            ret.stockSums = new List<StockSum>();
            List<Stock> stocksTmp = await _stockOperationRepo.ListBySPsAndWH(sods.Select(a => a.SpareParts).Distinct().ToList(), so.Warehouse);
            List<StockSum> stockSumsTmp = await _stockOperationRepo.ListBySPs(sods.Select(a => a.SpareParts).Distinct().ToList());

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
                if (sd.InStockNo == 0) sd.Status = (int)StockStatus.Exhaust;
                else if (sd.InStockNo > 1) sd.Status = (int)StockStatus.None;
                else sd.Status = (int)status;
                ret.stockDetails.Add(sd);
            }
            foreach (var item in sods.Select(a => a.SpareParts).Distinct())
            {
                var sod = sods.Where(a => a.SpareParts == item);
                int countNo = sod.Sum(a => a.CountNo);
                //应该需要换算字段，否则全部算是人民币
                double amount = sod.Sum(a => a.Amount);
                Stock s = stocksTmp.Where(a => a.SpareParts == item).FirstOrDefault();
                s.isAdd = false;
                s.StockNo = GetNo(stock, s.StockNo, countNo);
                s.TroubleNo = GetNo(trouble, s.TroubleNo, countNo);
                s.InStockNo = GetNo(inStock, s.InStockNo, countNo);
                s.InspectionNo = GetNo(inspection, s.InspectionNo, countNo);
                s.RepairNo = GetNo(repair, s.RepairNo, countNo);
                s.Amount += amount;
                ret.stocks.Add(s);

                StockSum ss = stockSumsTmp.Where(a => a.SpareParts == item).FirstOrDefault();
                ss.isAdd = false;
                ss.StockNo = GetNo(stock, ss.StockNo, countNo);
                ss.TroubleNo = GetNo(trouble, ss.TroubleNo, countNo);
                ss.InStockNo = GetNo(inStock, ss.InStockNo, countNo);
                ss.InspectionNo = GetNo(inspection, ss.InspectionNo, countNo);
                ss.RepairNo = GetNo(repair, ss.RepairNo, countNo);
                ss.Amount += amount;
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
        /// <summary>
        /// 库存操作保存专用
        /// </summary>
        /// <param name="msg"></param>
        private void GetApiResult(string msg)
        {
            validateSaveData.code = Code.CheckDataRulesFail;
            validateSaveData.msg = msg;
        }

        private async Task<StockOperationSaveParm> GetParmInit(StockOperationSaveParm parm)
        {
            StockOperationSaveParm ret = new StockOperationSaveParm();
            StockOperation so = parm.stockOperation;
            ret.stockOperation = so;
            List<StockOperationDetail> sods = JsonConvert.DeserializeObject<List<StockOperationDetail>>(so.DetailList);
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
                sd.InspectionNo = 0;
                sd.InStockNo = item.CountNo;
                sd.RepairNo = 0;
                sd.SpareParts = item.SpareParts;
                sd.Status = item.CountNo>1? (int)StockStatus.None:(int)StockStatus.Normal;
                sd.StockNo = item.CountNo;
                //sd.StockOperationDetail = item.ID;此ID需要在DAL层事务中赋值
                sd.TroubleNo = 0;
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
                if (reason== StockOptDetailType.PurchaseReceive || reason==StockOptDetailType.OtherReceive)
                {
                    s.DetailList = JsonConvert.SerializeObject(await _stockOperationRepo.ListByOperationIn(id));
                }
                else
                {
                    s.DetailList = JsonConvert.SerializeObject(await _stockOperationRepo.ListByOperationOut(id));
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

        #region 库存明细
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
        #endregion
    }
}
