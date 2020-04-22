using MSS.API.Common;
using MSS.API.Common.Utility;
using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using MSS.Common.Consul;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;

namespace MSS.API.Core.V1.Business
{
    public class LifeTimeKeyMaintainService : ILifeTimeKeyMaintainService
    {
        private readonly ILifeTimeKeyMaintainRepo<LifeTimeKeyMaintainInfo> _lifeTimeKeyMaintainRepo;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        private readonly int userID;

        public LifeTimeKeyMaintainService(ILifeTimeKeyMaintainRepo<LifeTimeKeyMaintainInfo> lifeTimeKeyMaintainRepo,
            IAuthHelper auth,
            IServiceDiscoveryProvider consulServiceProvider)
        {
            _lifeTimeKeyMaintainRepo = lifeTimeKeyMaintainRepo;
            _consulServiceProvider = consulServiceProvider;
            userID = auth.GetUserId();
        }
        public async Task<ApiResult> GetListByPage(string strWhere, string orderby, string sort, int page, int size)
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                   // int count = 10; //await _lifeTimeKeyMaintainRepo.GetRecordCount(strWhere);
                    var list = await _lifeTimeKeyMaintainRepo.GetListByPage(strWhere, sort, orderby, page, size);
                    if (list != null && list.Count() > 0)
                    {
                        foreach (var item in list)
                        {
                            item.setup_date = item.setup_date.Substring(0, 10);
                            item.life_time = item.life_time.Substring(0, 10);
                            item.next_maintain_date = item.next_maintain_date.Substring(0, 10);
                        }
                    }
                    ret.code = Code.Success;
                    ret.data = new
                    {
                        total = list.Count,
                        list = list
                    };

                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> GetdeviceList(string strWhere)
        {

            //id 有点问题,???/修正

            ApiResult ret = new ApiResult();
            try
            {  
                var list = await _lifeTimeKeyMaintainRepo.GetlocationList();
               List<LocatioInfo> locationinfolist = new List<LocatioInfo>();
                var ConfigTypeList = from v in  ((from m in list select new  { ID = m.ConfigTypeID, Name = m.ConfigType }).ToList().Distinct()) select new LocatioInfo { ID = v.ID, Name = v.Name };
               // var ConfigTypeList = from v in list0 select new LocatioInfo { ID = v.ID, Name = v.Name };
                foreach (var v in ConfigTypeList)
                { 
                    var list1 = from o in (( from o in list where o.ConfigTypeID==v.ID && o.StationName!=null
                                            select new   { Name = o.StationName, ID = o.StationID } ).ToList().Distinct()
                                            ) select new LocatioInfo { ID = o.ID, Name = o.Name };
                    var list11 = await _lifeTimeKeyMaintainRepo.GetdeviceList(1,v.ID, strWhere);
                    if (list1.Count() > 0 || list11.Count > 0)
                    {
                        if (list11.Count > 0)
                        {
                            v.Children = new List<LocatioInfo>() { };
                            v.Children.AddRange(list11);//添加设备
                            locationinfolist.Add(v);
                        } 
                        foreach (var m in list1)
                        { 
                            var list2 = from o in ((from o in list where o.StationID == m.ID && o.LocationName != null select new  { Name = o.LocationName, ID = o.LocationID }).ToList().Distinct())
                                        select new LocatioInfo { ID = o.ID, Name = o.Name };
                            var list22 = await _lifeTimeKeyMaintainRepo.GetdeviceList(2, m.ID, strWhere);
                            if (list2.Count() > 0 || list22.Count > 0)
                            {
                                if (list22.Count > 0)
                                {
                                    if (v.Children== null)
                                    {
                                        v.Children = new List<LocatioInfo>() { };
                                        locationinfolist.Add(v);
                                    }
                                    v.Children.Add(m);
                                    m.Children = new List<LocatioInfo>() { };
                                    m.Children.AddRange(list22);//添加设备 
                                } 
                                foreach (var u in list2)
                                {
                                    if (string.IsNullOrEmpty(u.ID.ToString()))
                                    {
                                        continue;
                                    }
                                    var list33 = await _lifeTimeKeyMaintainRepo.GetdeviceList(3, u.ID, strWhere);
                                    if (list33.Count > 0)
                                    {
                                        if (v.Children == null)
                                        {
                                            v.Children = new List<LocatioInfo>() { };
                                            locationinfolist.Add(v);
                                        }
                                        if(m.Children ==null)
                                        {
                                            m.Children = new List<LocatioInfo>() { };
                                        }
                                        v.Children.Add(m); 
                                        m.Children.Add(u);
                                        u.Children = new List<LocatioInfo>() { };
                                        u.Children.AddRange(list33); 
                                    }
                                }
                            }
                        }
                    }
                    
                }
                ret.code = Code.Success;
                ret.data = locationinfolist;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            } 
            return ret;
        }

        public async Task<ApiResult> ListCascaderByEqpTypeAndLine(int eqpType,int line)
        {
            ApiResult ret = new ApiResult();
            try
            {
                var list = await _lifeTimeKeyMaintainRepo.GetlocationList();
                List<LocatioInfo> locationinfolist = new List<LocatioInfo>();
                int? topOrg = await getTopOrgByUser();
                if (topOrg == 0)
                {
                    ret.code = Code.Failure;
                    ret.msg = "登录用户不归属于任何组织架构或所归属的组织架构下没有任何设备";
                    return ret;
                }
                List<Equipment> eqps = await _lifeTimeKeyMaintainRepo.ListEqpAllByCond(topOrg,eqpType,line);
                var ConfigTypeList = from v in ((from m in list select new { ID = m.ConfigTypeID, Name = m.ConfigType }).ToList().Distinct()) select new LocatioInfo { ID = v.ID, Name = v.Name };
                // var ConfigTypeList = from v in list0 select new LocatioInfo { ID = v.ID, Name = v.Name };
                foreach (var v in ConfigTypeList)
                {
                    var list1 = from o in ((from o in list
                                            where o.ConfigTypeID == v.ID && o.StationName != null
                                            select new { Name = o.StationName, ID = o.StationID }).ToList().Distinct()
                                            )
                                select new LocatioInfo { ID = o.ID, Name = o.Name };
                    if (list1.Count() > 0)
                    {
                        foreach (var m in list1)
                        {
                            var list2 = from o in ((from o in list where o.StationID == m.ID && o.LocationName != null select new { Name = o.LocationName, ID = o.LocationID }).ToList().Distinct())
                                        select new LocatioInfo { ID = o.ID, Name = o.Name };
                            var list22 = eqps.Where(a => a.LocationBy == 1 && a.Location == m.ID);
                            if (list2.Count() > 0 || list22.Count() > 0)
                            {
                                if (list22.Count() > 0)
                                {
                                    if (v.Children == null)
                                    {
                                        v.Children = new List<LocatioInfo>() { };
                                        locationinfolist.Add(v);
                                    }
                                    v.Children.Add(m);
                                    m.Children = new List<LocatioInfo>();
                                    LocatioInfo l = new LocatioInfo();
                                    var tmp = list22.FirstOrDefault();
                                    l.ID = tmp.ID;
                                    l.Name = tmp.Name;
                                    m.Children.Add(l);//添加设备 
                                }
                                foreach (var u in list2)
                                {
                                    if (string.IsNullOrEmpty(u.ID.ToString()))
                                    {
                                        continue;
                                    }
                                    var list33 = eqps.Where(a => a.LocationBy == 2 && a.Location == u.ID);
                                    if (list33.Count() > 0)
                                    {
                                        if (v.Children == null)
                                        {
                                            v.Children = new List<LocatioInfo>() { };
                                            locationinfolist.Add(v);
                                        }
                                        if (m.Children == null)
                                        {
                                            m.Children = new List<LocatioInfo>() { };
                                        }
                                        v.Children.Add(m);
                                        m.Children.Add(u);
                                        u.Children = new List<LocatioInfo>() { };
                                        LocatioInfo l = new LocatioInfo();
                                        var tmp = list33.FirstOrDefault();
                                        l.ID = tmp.ID;
                                        l.Name = tmp.Name;
                                        u.Children.Add(l);//添加设备 
                                    }
                                }
                            }
                        }
                    }

                }
                ret.code = Code.Success;
                ret.data = locationinfolist;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }

        private async Task<int?> getTopOrgByUser()
        {
            var _services = await _consulServiceProvider.GetServiceAsync("AuthService");
            IHttpClientHelper<ApiResult> h = new HttpClientHelper<ApiResult>();
            ApiResult r = await h.GetSingleItemRequest(_services + "/api/v1/user/" + userID);
            JObject jobj = JsonConvert.DeserializeObject<JObject>(r.data.ToString());
            if ((bool)jobj["is_super"])
            {
                return null;
            }
            else
            {
                IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                ApiResult result = await httpHelper.GetSingleItemRequest(_services + "/api/v1/org/topnode/" + userID);
                if (result.data != null)
                {
                    JObject obj = JsonConvert.DeserializeObject<JObject>(result.data.ToString());
                    return Convert.ToInt32(obj["id"]);
                }
                return 0;
            }
        }


        public async Task<ApiResult> GetListByPage(LifeTimeKeyMaintainQurey parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret.data=await _lifeTimeKeyMaintainRepo.GetListByPage(parm);
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }


    }
}
