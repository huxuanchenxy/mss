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
        public StockOperationService(IStockOperationRepo<StockOperation> stockOperationRepo, 
            IAuthHelper auth, IDistributedCache cache)
        {
            //_logger = logger;
            _stockOperationRepo = stockOperationRepo;
            _cache = cache;
            userID = auth.GetUserId();
        }
        #region 库存操作
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
            List<StockDetailEdit> sdes = JsonConvert.DeserializeObject<List<StockDetailEdit>>(so.DetailEditList);

            ret.isAddStockDetails = false;
            ret.stockDetails = new List<StockDetail>();
            ret.stocks = new List<Stock>();
            ret.stockSums = new List<StockSum>();
            switch (stock)
            {
                case StockChange.NoChange:
                    break;
            }
            return ret;
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
                sd.Status = (int)StockStatus.Normal;
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
                s.DetailList = JsonConvert.SerializeObject(await _stockOperationRepo.ListByOperation(id));
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
                StockSumView ssv = await _stockOperationRepo.GetStockSumPageByParm(parm);
                List<Stock> stocks = await _stockOperationRepo.ListStockBySPs(ssv.rows.Select(a => a.SpareParts).Distinct().ToList());
                foreach (var item in ssv.rows)
                {
                    var tmp = stocks.Where(a => a.SpareParts == item.SpareParts);
                    item.stocks = tmp==null?null:tmp.ToList();
                }
                ret.data = ssv;
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
        public async Task<ApiResult> ListStockDetailBySPs(int spareParts)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _stockOperationRepo.ListStockDetailBySPs(new List<int> { spareParts });
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
