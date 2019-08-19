using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MSS.API.Model.DTO;

namespace MSS.API.Dao.Interface
{
    public interface IStatisticsRepo
    {
        Task<StatisticsAlarm> SaveStatisticsAlarm(StatisticsAlarm alarm);
    }
}
