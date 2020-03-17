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
    public interface IHealthConfigService
    {
        Task<ApiResult> UpdateList(List<HealthConfig> hConfigs);
        Task<ApiResult> Update(HealthConfig hConfig);
        Task<ApiResult> Delete(int type, int? eqpType);
        Task<ApiResult> GetPageByParm(HealthConfigQueryParm parm);
        Task<ApiResult> GetByType(int type);
        Task<ApiResult> GetByTroubleLevel(int type, int eqpType);
    }
    public class HealthConfigService : IHealthConfigService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IHealthConfigRepo<HealthConfig> _healthConfigRepo;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;

        private readonly int userID;

        public HealthConfigService(IHealthConfigRepo<HealthConfig> healthConfigRepo,
            IAuthHelper auth, IServiceDiscoveryProvider consulServiceProvider)
        {
            //_logger = logger;
            _healthConfigRepo = healthConfigRepo;
            _consulServiceProvider = consulServiceProvider;
            userID = auth.GetUserId();
        }

        public async Task<ApiResult> UpdateList(List<HealthConfig> hConfigs)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                foreach (var item in hConfigs)
                {
                    if (item.ID == 0)
                    {
                        item.CreatedTime = dt;
                        item.CreatedBy = userID;
                    }
                    item.UpdatedTime = dt;
                    item.UpdatedBy = userID;
                }
                if (hConfigs[0].ID == 0)
                {
                    ret.data = await _healthConfigRepo.Save(hConfigs);
                }
                else
                {
                    ret.data = await _healthConfigRepo.Update(hConfigs);
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResult> Update(HealthConfig hConfig)
        {
            ApiResult ret = new ApiResult();
            try
            {
                //HealthConfig hc = await _healthConfigRepo.GetByID(hConfig.Type);
                DateTime dt = DateTime.Now;
                hConfig.UpdatedTime = dt;
                hConfig.UpdatedBy = userID;
                if (hConfig.ID != 0)
                {
                    ret.data = await _healthConfigRepo.Update(hConfig);
                }
                else
                {
                    hConfig.CreatedTime = dt;
                    hConfig.CreatedBy = userID;
                    ret.data = await _healthConfigRepo.Save(hConfig);
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

        public async Task<ApiResult> Delete(int type,int? eqpType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _healthConfigRepo.Delete(type, eqpType);
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResult> GetPageByParm(HealthConfigQueryParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows = parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                HealthConfigView etv = await _healthConfigRepo.GetPageByParm(parm);
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

        public async Task<ApiResult> GetByType(int type)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _healthConfigRepo.GetByType(type);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }
        public async Task<ApiResult> GetByTroubleLevel(int type, int eqpType)
        {
            ApiResult ret = new ApiResult();
            string typeName, des;
            try
            {
                List<HealthConfig> data= await _healthConfigRepo.GetByTroubleLevel(type, eqpType);
                if (data[0].Type == 0)
                {
                    HealthConfig hc = await _healthConfigRepo.GetByType(type);
                    typeName = hc.TypeName;
                    des = hc.Des;
                }
                else
                {
                    typeName = data[0].TypeName;
                    des = data[0].Des;
                }
                ret.data = new { typeName, des, data };
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
