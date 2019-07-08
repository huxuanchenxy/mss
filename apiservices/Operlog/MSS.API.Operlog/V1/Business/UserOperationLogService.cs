using MSS.API.Operlog.Dao.Interface;
using MSS.API.Operlog.Model.Data;
using System.Threading.Tasks;
using MSS.API.Operlog.Model.DTO;
using static MSS.API.Operlog.Model.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using MSS.API.Common;

namespace MSS.API.Operlog.V1.Business
{
    public class UserOperationLogService : IUserOperationLogService
    {
        //private readonly ILogger<ActionService> _logger;
        private readonly IUserOperationLogRepo<UserOperationLog> _userOperationLogRepo;

        public UserOperationLogService(IUserOperationLogRepo<UserOperationLog> userOperationLogRepo)
        {
            //_logger = logger;
            _userOperationLogRepo = userOperationLogRepo;
        }
        public async Task<MSSResult<UserOperationLogView>> GetPageByParm(UserOperationLogParm parm)
        {
            MSSResult<UserOperationLogView> mRet = new MSSResult<UserOperationLogView>();
            try
            {
                parm.page = parm.page == 0 ? 1 : parm.page;
                parm.rows= parm.rows == 0 ? PAGESIZE : parm.rows;
                parm.sort = string.IsNullOrWhiteSpace(parm.sort) ? "id" : parm.sort;
                parm.order = parm.order.ToLower() == "desc" ? "desc" : "asc";
                mRet = await _userOperationLogRepo.GetPageByParm(parm);
                mRet.code = (int)ErrType.OK;
                return mRet;
            }
            catch (Exception ex)
            {
                mRet.code = (int)ErrType.SystemErr;
                mRet.msg = ex.Message;
                return mRet;
            }
        }


        public async Task<int> Add(UserOperationLog obj)
        {
            return await _userOperationLogRepo.Add(obj);
        }

        public async Task<ApiResult> GetUserOperationLog(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                var data = await _userOperationLogRepo.GetUserOperationLog(id);
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

    }
}
