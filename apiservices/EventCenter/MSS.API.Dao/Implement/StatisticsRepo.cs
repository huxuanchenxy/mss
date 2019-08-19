using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using MSS.API.Dao;
using MSS.API.Model.Data;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using MSS.API.Model.DTO;

namespace MSS.API.Dao.Implement
{
    public class StatisticsRepo : BaseRepo, IStatisticsRepo
    {
        public StatisticsRepo(DapperOptions options) : base(options) { }

        public async Task<StatisticsAlarm> SaveStatisticsAlarm(StatisticsAlarm alarm)
        {
            return await WithConnection(async c =>
            {
                string sql = "INSERT INTO statistics_alarm (eqp_id, level, occur_time, recover_time, elapsed_time, prop, content)"
                            + " Values (@EqpID, @Level, @OccurTime, @RecoverTime, @ElapsedTime, @Prop, @Content);";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, alarm);
                alarm.ID = newid;
                return alarm;
            });
        }
    }
}
