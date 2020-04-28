using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MSS.API.Model.DTO;
using MSS.API.Dao.Common;
namespace MSS.API.Dao.Interface
{
    public interface IStatisticsRepo
    {
        // 报警统计
        Task<List<StatisticsAlarm>> ListStatisticsAlarmByDate(StatisticsParam param,
            List<string> groupby, int dateType, bool defaultByDate = true);

        // 查询报警列表 不做分组统计
        Task<List<StatisticsAlarm>> ListStatisticsAlarmDetail(StatisticsParam param);

        // 故障统计
        Task<List<StatisticsTrouble>> ListStatisticsTrouble(StatisticsTroubleParam param,
            List<string> groupby, int dateType);
        // 查询故障列表 不做分组统计
        Task<List<StatisticsTrouble>> ListStatisticsTroubleDetail(StatisticsTroubleParam param);

        // 添加故障
        Task<StatisticsTrouble> AddTrouble(StatisticsTrouble trouble);
        //主页故障排名
        Task<List<StatisticsTroubleRank>> GetStatisticsTroubleRank();
        //主页成本分摊
        Task<List<CostChart>> GetCostChart(CostChartParm parm);
    }
}
