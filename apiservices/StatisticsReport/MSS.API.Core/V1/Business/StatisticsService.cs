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
using MSS.API.Core.Common;
using Microsoft.Extensions.Caching.Distributed;
using MSS.Common.Consul;
using MSS.API.Common.Utility;
namespace MSS.API.Core.V1.Business
{
    public class StatisticsService : IStatisticsService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IStatisticsRepo _statisticsRepo;

        public StatisticsService(IStatisticsRepo statisticsRepo)
        {
            _statisticsRepo = statisticsRepo;
        }

        public async Task<ApiResult> ListStatisticsAlarm(StatisticsParam param,
            List<string> groupby, int dateType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<StatisticsAlarm> data = await _statisticsRepo.ListStatisticsAlarmByDate(param,
                    groupby, dateType);
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

        public async Task<ApiResult> ListStatisticsAlarmGroupByEqpType(StatisticsParam param, int dateType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<StatisticsAlarm> data = await _statisticsRepo.ListStatisticsAlarmByDate(param,
                    new List<string> { "eqp_type_id" }, dateType, false);
                
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

        public async Task<ApiResult> ListStatisticsAlarmGroupBySupplier(StatisticsParam param, int dateType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<StatisticsAlarm> data = await _statisticsRepo.ListStatisticsAlarmByDate(param,
                    new List<string> { "supplier_id" }, dateType);

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

        public async Task<ApiResult> ListStatisticsAlarmGroupByManufacturer(StatisticsParam param, int dateType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<StatisticsAlarm> data = await _statisticsRepo.ListStatisticsAlarmByDate(param,
                    new List<string> { "manufacturer_id" }, dateType);

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

        public async Task<ApiResult> ListStatisticsAlarmGroupBySubSystem(StatisticsParam param, int dateType)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<StatisticsAlarm> data = await _statisticsRepo.ListStatisticsAlarmByDate(param,
                    new List<string> { "sub_system_id" }, dateType);

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
