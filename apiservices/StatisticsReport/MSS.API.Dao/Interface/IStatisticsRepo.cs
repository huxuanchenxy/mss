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
        Task<List<StatisticsAlarm>> ListStatisticsAlarmByDate(StatisticsParam param,
            List<string> groupby, int dateType, bool defaultByDate = true);
    }
}
