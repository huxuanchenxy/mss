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
    }
}
