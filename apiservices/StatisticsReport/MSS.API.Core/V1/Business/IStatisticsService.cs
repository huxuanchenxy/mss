using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Common;
using MSS.API.Model.DTO;
namespace MSS.API.Core.V1.Business
{
    public interface IStatisticsService
    {
        Task<ApiResult> ListStatisticsAlarm(StatisticsParam param,
            List<string> groupby, int dateType);
        Task<ApiResult> ListStatisticsAlarmGroupByEqpType(StatisticsParam param, int dateType);
        Task<ApiResult> ListStatisticsAlarmGroupBySupplier(StatisticsParam param, int dateType);
        Task<ApiResult> ListStatisticsAlarmGroupByManufacturer(StatisticsParam param, int dateType);
        Task<ApiResult> ListStatisticsAlarmGroupBySubSystem(StatisticsParam param, int dateType);
        Task<ApiResult> ListStatisticsAlarmGroupByLocation(StatisticsParam param, int dateType);
        Task<ApiResult> ListStatisticsAlarmGroupByOrg(StatisticsParam param, int dateType);
        Task<ApiResult> GetStatisticsTroubleRank();
        Task<ApiResult> GetRunningtime();
        Task<ApiResult> GetIndexProcess();
    }
}
