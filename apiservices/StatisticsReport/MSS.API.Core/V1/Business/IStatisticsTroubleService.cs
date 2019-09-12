using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Common;
using MSS.API.Model.DTO;
namespace MSS.API.Core.V1.Business
{
    public interface IStatisticsTroubleService
    {
        // 默认日期统计
        Task<ApiResult> ListStatisticsTroubleGroupByDate(StatisticsTroubleParam param,
            List<string> groupby, int dateType);
        Task<ApiResult> ListStatisticsTroubleGroupByOther(StatisticsTroubleParam param,
            List<string> groupby, int dateType);
        Task<ApiResult> ListStatisticsTroubleGroupByLocation(StatisticsTroubleParam param, int dateType);
        Task<ApiResult> ListStatisticsTroubleGroupByOrg(StatisticsTroubleParam param, int dateType);

        Task<ApiResult> AddTrouble(StatisticsTrouble trouble);
    }
}
