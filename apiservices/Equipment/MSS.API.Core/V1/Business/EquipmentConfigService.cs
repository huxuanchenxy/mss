using CSRedis;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using MSS.API.Common;
using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using static MSS.API.Common.Const;

namespace MSS.API.Core.V1.Business
{
    public class EquipmentConfigService : IEquipmentConfigService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IEquipmentConfigRepo<EquipmentConfig> _equipmentRepo;
        private readonly IDistributedCache _cache;
        private IConfiguration _configuration { get; }

        public EquipmentConfigService(IEquipmentConfigRepo<EquipmentConfig> equipmentRepo, IDistributedCache cache, IConfiguration configuration)
        {
            //_logger = logger;
            _equipmentRepo = equipmentRepo;
            _cache = cache;
            _configuration = configuration;
        }

        public async Task<ApiResult> Save(EquipmentConfig obj)
        {
            ApiResult ret = new ApiResult();
            try
            {
                //var data = await _equipmentRepo.GetAll();
                //if (data != null && data.Count > 0)
                //{
                //    throw new Exception("只能添加一条配置");
                //}
                DateTime dt = DateTime.Now;
                obj.UpdatedTime = dt;
                obj.CreatedTime = dt;
                ret.data = await _equipmentRepo.Save(obj);
                SyncRedis(obj);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        private void SyncRedis(EquipmentConfig obj)
        {
            var redisconfig = _configuration["redis_EQPConfig:ConnectionString"];
            using (var redis = new CSRedisClient(redisconfig))
            {
                var key = _configuration["redis_EQPConfig:Instance"];
                redis.Set(key, JsonConvert.SerializeObject(obj));
            }
        }

        public async Task<ApiResult> Update(EquipmentConfig obj)
        {
            ApiResult ret = new ApiResult();
            try
            {
                EquipmentConfig et = await _equipmentRepo.GetByID(obj.ID);
                if (et!=null)
                {
                    DateTime dt = DateTime.Now;
                    obj.UpdatedTime = dt;
                    ret.data = await _equipmentRepo.Update(obj);
                    SyncRedis(obj);
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

        public async Task<ApiResult> Delete(string ids,int userID)
        {
            ApiResult ret = new ApiResult();
            try
            {
                // 判断设备类型下有没有挂设备，有的话不允许删除
                ret.data = await _equipmentRepo.Delete(ids.Split(','),userID);
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
                ret.data = await _equipmentRepo.GetByID(id);
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
                ret.data = await _equipmentRepo.GetAll();
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> GetPageByParm(EquipmentConfigParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows = parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                ret.data = await _equipmentRepo.GetPageByParm(parm);
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
