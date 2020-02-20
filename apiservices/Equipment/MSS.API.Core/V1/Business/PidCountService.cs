﻿using MSS.API.Common;
using MSS.API.Common.Utility;
using MSS.API.Dao.Implement;
using MSS.API.Model.Data;
using System;
using System.Net;
using System.Threading.Tasks;


// Coded By admin 2020/2/17 14:25:01
namespace MSS.API.Core.V1.Business
{
    public interface IPidCountService
    {
        Task<ApiResult> GetPageList(PidCountParm parm);
        Task<ApiResult> Save(PidCount obj);
        Task<ApiResult> Update(PidCount obj);
        Task<ApiResult> Delete(string ids);
        Task<ApiResult> GetByID(int id);
    }

    public class PidCountService : IPidCountService
    {
        private readonly IPidCountRepo<PidCount> _repo;
        private readonly IAuthHelper _authhelper;
        private readonly int _userID;
        private readonly IPidCountDetailService _serviceDetail;
        private readonly INotificationPidcountService _serviceNotice;

        public PidCountService(IPidCountRepo<PidCount> repo, IAuthHelper authhelper, IPidCountDetailService serviceDetail, INotificationPidcountService serviceNotice)
        {
            _repo = repo;
            _authhelper = authhelper;
            _userID = _authhelper.GetUserId();
            _serviceDetail = serviceDetail;
            _serviceNotice = serviceNotice;
        }

        public async Task<ApiResult> GetPageList(PidCountParm parm)
        {
            ApiResult ret = new ApiResult();
            try
            {
                //parm.UserID = _userID;
                //parm.UserID = 40;
                var data = await _repo.GetPageList(parm);
                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> Save(PidCount obj)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                obj.UpdatedTime = dt;
                obj.CreatedTime = dt;
                obj.UpdatedBy = _userID;
                obj.CreatedBy = _userID;
                obj.RemainCount = obj.CapacityCount - obj.UsedCount;
                ret.data = await _repo.Save(obj);
                ret.code = Code.Success;
                return ret;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
                return ret;
            }
        }

        public async Task<ApiResult> Update(PidCount obj)
        {
            ApiResult ret = new ApiResult();
            try
            {
                PidCount et = await _repo.GetByID(obj.ID);
                if (et != null)
                {
                    DateTime dt = DateTime.Now;
                    obj.UpdatedTime = dt;
                    obj.UpdatedBy = _userID;
                    obj.RemainCount = obj.CapacityCount - obj.UsedCount;
                    ret.data = await _repo.Update(obj);
                    PidCountDetail obj1 = new PidCountDetail()
                    {
                        PidCountId = obj.ID,
                        CapacityCountOld = et.CapacityCount,
                        CapacityCountNew = obj.CapacityCount,
                        UsedCountOld = et.UsedCount,
                        UsedCountNew = obj.UsedCount,
                        RemainCountOld = et.RemindCount,
                        RemainCountNew = obj.RemindCount,
                        RemindCountOld = et.RemindCount,
                        RemindCountNew = obj.RemindCount,
                        CreatedTime = dt,
                        CreatedBy = _userID,
                        UpdatedTime = dt,
                        UpdatedBy = _userID
                    };
                    await _serviceDetail.Save(obj1);
                    //判断预警
                    if (obj.UsedCount > obj.RemindCount)
                    {
                        await _serviceNotice.Save(new NotificationPidcount()
                        {
                            PidCountId = obj.ID,
                            PidCountName = obj.NodeName,
                            Content = "超过了预设的点位容量" + obj.RemindCount + "个点"
                            , IsDel = false, CreatedTime = dt,
                            CreatedBy  = _userID,
                            UpdatedTime = dt,
                            UpdatedBy = _userID,
                            Status = 0
                        });
                    }
                    ret.code = Code.Success;
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
                ret.data = await _repo.Delete(ids.Split(','), _userID);
                ret.code = Code.Success;
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
                PidCount obj = await _repo.GetByID(id);
                ret.data = obj;
                ret.code = Code.Success;
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



