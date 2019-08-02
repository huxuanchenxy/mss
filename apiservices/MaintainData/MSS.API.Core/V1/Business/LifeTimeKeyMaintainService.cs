using MSS.API.Common;
using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
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

        public LifeTimeKeyMaintainService(ILifeTimeKeyMaintainRepo<LifeTimeKeyMaintainInfo> lifeTimeKeyMaintainRepo)
        {
            _lifeTimeKeyMaintainRepo = lifeTimeKeyMaintainRepo;
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
    }
}
