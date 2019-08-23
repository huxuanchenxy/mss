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

        public async Task<ApiResult> Save(StockOperation stockOperation)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                stockOperation.CreatedTime = dt;
                stockOperation.CreatedBy = userID;
                StockOperationType t = (StockOperationType)stockOperation.Type;
                string redisKey = GetRedisKey(t);
                long tmp = Convert.ToInt64(_cache.GetString(redisKey)) + 1;
                stockOperation.OperationID = GetOperationID(tmp, t);
                ret.data = await _stockOperationRepo.Save(stockOperation);
                if (ret.data != null)
                {
                    _cache.SetString(redisKey, tmp.ToString());
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

    }
}
