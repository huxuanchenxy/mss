using MSS.API.Dao.Interface;
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
using MSS.API.Dao.Common;

namespace MSS.API.Core.V1.Business
{
    public class WarnningService : IWarnningService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IWarnningRepo<EarlyWarnning> _warnRepo;

        public WarnningService(IWarnningRepo<EarlyWarnning> warnRepo)
        {
            //_logger = logger;
            _warnRepo = warnRepo;
        }

        public async Task<ApiResult> ListWarnningByOrg(int? orgID)
        {
            ApiResult ret = new ApiResult();
            try
            {
                var data = await _warnRepo.ListEarlyWarnningByOrg(orgID);
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

        public async Task<ApiResult> ListNotificationByOrg(int? orgID)
        {
            ApiResult ret = new ApiResult();
            try
            {
                var data = await _warnRepo.ListNotificationByOrg(orgID);
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

        public async Task<ApiResult> ListEarlyWarningHistory(WarningParam param)
        {
            ApiResult ret = new ApiResult();
            try
            {
                var data = await _warnRepo.ListEarlyWarningHistory(param);
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

        public async Task<ApiResult> ListNotificationHistory(NotificationParam param)
        {
            ApiResult ret = new ApiResult();
            try
            {
                var data = await _warnRepo.ListNotificationHistory(param);
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
