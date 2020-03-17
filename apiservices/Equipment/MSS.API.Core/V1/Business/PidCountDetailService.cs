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

        public PidCountDetailService(IPidCountDetailRepo<PidCountDetail> repo, IAuthHelper authhelper)
        {
            _repo = repo;
            _authhelper = authhelper;
            _userID = _authhelper.GetUserId();
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



