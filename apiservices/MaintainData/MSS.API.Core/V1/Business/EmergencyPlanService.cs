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

namespace MSS.API.Core.V1.Business
{
    public class EmergencyPlanService : IEmergencyPlanService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IEmergencyPlanRepo<EmergencyPlan> _emergencyPlanRepo;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;

        private readonly int userID;

        public EmergencyPlanService(IEmergencyPlanRepo<EmergencyPlan> emergencyPlanRepo,
            IAuthHelper auth, IServiceDiscoveryProvider consulServiceProvider)
        {
            //_logger = logger;
            _emergencyPlanRepo = emergencyPlanRepo;
            _consulServiceProvider = consulServiceProvider;
            userID = auth.GetUserId();
        }

        public async Task<ApiResult> Save(EmergencyPlan ePlan)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                ePlan.UpdatedTime = dt;
                ePlan.CreatedTime = dt;
                ePlan.UpdatedBy = userID;
                ePlan.CreatedBy = userID;
                ret.data = await _emergencyPlanRepo.Save(ePlan);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> Update(EmergencyPlan ePlan)
        {
            ApiResult ret = new ApiResult();
            try
            {
                EmergencyPlan et = await _emergencyPlanRepo.GetByID(ePlan.ID);
                if (et!=null)
                {
                    DateTime dt = DateTime.Now;
                    ePlan.UpdatedTime = dt;
                    ePlan.UpdatedBy = userID;
                    ret.data = await _emergencyPlanRepo.Update(ePlan);
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
                ret.data = await _emergencyPlanRepo.Delete(ids.Split(','));
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }

        public async Task<ApiResult> GetPageByParm(EPlanQueryParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows = parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                EPlanView etv = await _emergencyPlanRepo.GetPageByParm(parm);
                if (etv.total != 0)
                {
                    string ids = String.Join(",", etv.rows.Select(a => a.ID));
                    List<UploadFilesEntity> jarr = await UploadFileHelper.GetUploadFile(ids,
                    UploadShowType.Cascader, _consulServiceProvider, SystemResource.EmergencyPlan);
                    if (jarr != null && jarr.Count() > 0)
                    {
                        foreach (var item in etv.rows)
                        {
                            var tmp = jarr.Where(a => a.Entity == item.ID);
                            if (tmp.Count() > 0)
                            {
                                item.UploadFiles = tmp.FirstOrDefault().UploadFiles;
                            }
                        }
                    }
                }
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

        public async Task<ApiResult> GetByID(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                EmergencyPlan et = await _emergencyPlanRepo.GetByID(id);
                string ids = id.ToString();
                //List<UploadFilesEntity> jarr = await GetUploadFile(ids,UploadShowType.List);
                List<UploadFilesEntity> jarr = await UploadFileHelper.GetUploadFile(ids, 
                    UploadShowType.List, _consulServiceProvider, SystemResource.EmergencyPlan);
                if (jarr != null && jarr.Count() > 0)
                {
                    et.UploadFiles= jarr.FirstOrDefault().UploadFiles;
                }
                ret.data = et;
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
                ret.data = await _emergencyPlanRepo.GetAll();
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        //private async Task<List<UploadFilesEntity>> GetUploadFile(string ids, UploadShowType ust)
        //{
        //    var _services = await _consulServiceProvider.GetServiceAsync("EqpService");
        //    IHttpClientHelper<ApiResult> h = new HttpClientHelper<ApiResult>();
        //    //string ids = String.Join(",", etv.rows.Select(a => a.ID));
        //    string url = "/api/v1/Upload/ListByEntity/" + ids + "/" + (int)SystemResource.EmergencyPlan + "/" + (int)ust;
        //    ApiResult r = await h.GetSingleItemRequest(_services + url);
        //    return JsonConvert.DeserializeObject<List<UploadFilesEntity>>(r.data.ToString());
        //}

        //private class UploadFilesEntity
        //{
        //    public int Entity { get; set; }
        //    public string UploadFiles { get; set; }
        //}
    }
}
