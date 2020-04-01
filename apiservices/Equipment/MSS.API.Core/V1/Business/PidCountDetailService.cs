using MSS.API.Common;
using MSS.API.Common.Utility;
using System;
using System.Net;
using System.Threading.Tasks;
using MSS.API.Model.Data;
using MSS.API.Dao.Implement;


// Coded By admin 2020/2/19 9:52:49
namespace MSS.API.Core.V1.Business
{
    public interface IPidCountDetailService
    {
        Task<ApiResult> GetPageList(PidCountDetailParm parm);
        Task<ApiResult> Save(PidCountDetail obj);
        Task<ApiResult> Update(PidCountDetail obj);
        Task<ApiResult> Delete(string ids);
        Task<ApiResult> GetByID(int id);
    }

    public class PidCountDetailService : IPidCountDetailService
    {
        private readonly IPidCountDetailRepo<PidCountDetail> _repo;
        private readonly IAuthHelper _authhelper;
        private readonly int _userID;
        private readonly IPidCountRepo<PidCount> _repoPidCount;
        private readonly INotificationPidcountService _serviceNotice;

        public PidCountDetailService(IPidCountDetailRepo<PidCountDetail> repo, IAuthHelper authhelper, IPidCountRepo<PidCount> repoPidCount, INotificationPidcountService serviceNotice)
        {
            _repo = repo;
            _authhelper = authhelper;
            _userID = _authhelper.GetUserId();
            _repoPidCount = repoPidCount;
            _serviceNotice = serviceNotice;
        }

        public async Task<ApiResult> GetPageList(PidCountDetailParm parm)
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

        public async Task<ApiResult> Save(PidCountDetail obj)
        {
            ApiResult ret = new ApiResult();
            try
            {
                DateTime dt = DateTime.Now;
                obj.UpdatedTime = dt;
                obj.CreatedTime = dt;
                obj.UpdatedBy = _userID;
                obj.CreatedBy = _userID;
                PidCount et = await _repoPidCount.GetByID(obj.PidCountId);
                if (et != null)
                {
                    if (obj.DetailType == 1)
                    {
                        obj.CountOld = et.CapacityCount;
                        obj.CountNew = et.CapacityCount + obj.ChangeCount;
                        et.CapacityCount = obj.CountNew;
                    }
                    else if (obj.DetailType == 2)
                    {
                        obj.CountOld = et.CapacityCount;
                        obj.CountNew = et.CapacityCount - obj.ChangeCount;
                        et.CapacityCount = obj.CountNew;
                    }
                    else if (obj.DetailType == 3) {
                        obj.CountOld = et.UsedCount;
                        obj.CountNew = et.UsedCount + obj.ChangeCount;
                        et.UsedCount = obj.CountNew;
                    }
                    else if (obj.DetailType == 4)
                    {
                        obj.CountOld = et.UsedCount;
                        obj.CountNew = et.UsedCount - obj.ChangeCount;
                        et.UsedCount = obj.CountNew;
                    }

                    et.RemainCount = et.CapacityCount - et.UsedCount;
                    ret.data = await _repo.Save(obj);
                    et.UpdatedTime = dt;
                    et.UpdatedBy = _userID;
                    await _repoPidCount.Update(et);

                    //判断预警
                    if (et.UsedCount > et.RemindCount)
                    {
                        long _cont = et.UsedCount - et.RemindCount;
                        NotificationPidcount nobj = new NotificationPidcount()
                        {
                            PidCountId = et.ID,
                            PidCountName = et.NodeName,
                            Content = "超过了预设的点位容量 " + _cont + " 个点",
                            IsDel = false,
                            CreatedTime = dt,
                            CreatedBy = _userID,
                            UpdatedTime = dt,
                            UpdatedBy = _userID,
                            Status = 0
                        };
                        await _serviceNotice.UpdateOtherPidCount(new NotificationPidcount() { PidCountId = et.ID});//先把当前车站的之前的预警状态更新成系统处理
                        await _serviceNotice.Save(nobj);//保证一个车站只有一条status=0
                    }
                }
                
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

        public async Task<ApiResult> Update(PidCountDetail obj)
        {
            ApiResult ret = new ApiResult();
            try
            {
                PidCountDetail et = await _repo.GetByID(obj.ID);
                if (et != null)
                {
                    DateTime dt = DateTime.Now;
                    obj.UpdatedTime = dt;
                    obj.UpdatedBy = _userID;
                    ret.data = await _repo.Update(obj);
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
                PidCountDetail obj = await _repo.GetByID(id);
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



