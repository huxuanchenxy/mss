﻿using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System.Threading.Tasks;
using MSS.API.Model.DTO;
using static MSS.API.Model.Const;
using System;
using System.Collections.Generic;
using System.Linq;

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

    }
}
