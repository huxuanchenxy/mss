using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using MSS.API.Common;
using static MSS.API.Common.Const;
using MSS.API.Common.Utility;
using MSS.Common.Consul;
using static MSS.API.Common.MyDictionary;
using Newtonsoft.Json.Linq;
using MSS.API.Dao.Implement;
using System.Transactions;

namespace MSS.API.Core.V1.Business
{
    public interface IHealthService
    {
        Task<ApiResult> GetPageByParm(HealthQueryParm parm);
    }
    public class HealthService : IHealthService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IHealthRepo<Health> _healthRepo;

        private readonly int userID;

        public HealthService(IHealthRepo<Health> healthRepo,
            IAuthHelper auth)
        {
            //_logger = logger;
            _healthRepo = healthRepo;
            userID = auth.GetUserId();
        }

        public async Task<ApiResult> GetPageByParm(HealthQueryParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows = parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                HealthView etv = await _healthRepo.GetPageByParm(parm);
                ret.data = etv;
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
