﻿using MSS.API.Dao.Interface;
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
using MSS.API.Common.Utility;
using System.IO;
using MSS.Common.Consul;

namespace MSS.API.Core.V1.Business
{
    public class EquipmentService :IEquipmentService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IEquipmentRepo<Equipment> _eqpRepo;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;

        private readonly int userID;

        public EquipmentService(IEquipmentRepo<Equipment> eqpRepo, IAuthHelper auth, IServiceDiscoveryProvider consulServiceProvider)
        {
            //_logger = logger;
            _eqpRepo = eqpRepo;
            _consulServiceProvider = consulServiceProvider;
            userID = auth.GetUserId();
        }

        public async Task<ApiResult> Save(Equipment eqp)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                eqp.UpdatedTime = dt;
                eqp.CreatedTime = dt;
                eqp.UpdatedBy = userID;
                eqp.CreatedBy = userID;
                ret.data = await _eqpRepo.Save(eqp);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> Update(Equipment eqp)
        {
            ApiResult ret = new ApiResult();
            try
            {
                Equipment et = await _eqpRepo.GetByID(eqp.ID);
                if (et!=null)
                {
                    DateTime dt = DateTime.Now;
                    eqp.UpdatedTime = dt;
                    eqp.UpdatedBy = userID;
                    ret.data = await _eqpRepo.Update(eqp);
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
                ret.data = await _eqpRepo.Delete(ids.Split(','),userID);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> GetPageByParm(EqpQueryParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                parm.SearchTopOrg= await getTopOrgByUser();
                if (parm.SearchTopOrg != 0)
                {
                    parm.page = parm.page == 0 ? 1 : parm.page;
                    parm.rows = parm.rows == 0 ? PAGESIZE : parm.rows;
                    parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                    parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                    EqpView ev = await _eqpRepo.GetPageByParm(parm);
                    List<Equipment> eqps = ev.rows;
                    List<AllArea> laa = await _eqpRepo.GetAllArea();
                    foreach (var item in eqps)
                    {
                        item.LocationName = laa.Where(a => a.Tablename == item.LocationBy && a.ID == item.Location)
                            .FirstOrDefault().AreaName;
                    }
                    ret.data = ev;
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
        public async Task<ApiResult> GetByID(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                Equipment e = await _eqpRepo.GetByID(id);
                var list = await _eqpRepo.ListByEqp(e.ID);
                if (list != null)
                {
                    e.FileIDs = string.Join(",", list.Select(a => a.FileID).ToArray());
                    ret.data = e;
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
        public async Task<ApiResult> GetDetailByID(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                Equipment e = await _eqpRepo.GetDetailByID(id);
                List<AllArea> laa = await _eqpRepo.GetAllArea();
                e.LocationName = laa.Where(a => a.Tablename == e.LocationBy && a.ID == e.Location)
                    .FirstOrDefault().AreaName;
                var list = await _eqpRepo.ListByEqp(e.ID);
                if (list!=null)
                {
                    e.FileIDs = string.Join(",", list.Select(a => a.FileID).ToArray());
                    ret.data = e;
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

        public async Task<ApiResult> ListByPosition(int location, int locationBy,int eqpType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                int topOrg = await getTopOrgByUser();
                if (topOrg != 0)
                {
                    ret.data = await _eqpRepo.ListByPosition(location, locationBy, eqpType, topOrg);
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
        public async Task<ApiResult> GetAll()
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data = await _eqpRepo.GetAll();
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        private async Task<int> getTopOrgByUser()
        {
            var _services = await _consulServiceProvider.GetServiceAsync("AuthService");
            IHttpClientHelper<ApiResult> h = new HttpClientHelper<ApiResult>();
            ApiResult r = await h.GetSingleItemRequest(_services + "/api/v1/user/" + userID);
            JObject jobj = JsonConvert.DeserializeObject<JObject>(r.data.ToString());
            if (!(bool)jobj["is_super"])
            {
                IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                ApiResult result = await httpHelper.GetSingleItemRequest(_services + "/api/v1/org/topnode/" + userID);
                if (result.data != null)
                {
                    JObject obj = JsonConvert.DeserializeObject<JObject>(result.data.ToString());
                    return Convert.ToInt32(obj["id"]);
                }
            }
            return 0;
        }
    }
}
