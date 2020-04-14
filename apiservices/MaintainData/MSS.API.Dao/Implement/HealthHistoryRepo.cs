using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using MSS.API.Dao;
using MSS.API.Model.Data;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using MSS.API.Common;
using System.Data;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static MSS.API.Common.MyDictionary;

namespace MSS.API.Dao.Implement
{
    public interface IHealthHistoryRepo<T>
    {
        Task<int> Save(HealthHistory healthHistory);
        Task<HealthHistoryView> GetPageByParm(HealthHistoryQueryParm parm);
    }

    public class HealthHistoryRepo : BaseRepo, IHealthHistoryRepo<HealthHistory>
    {
        public HealthHistoryRepo(DapperOptions options) : base(options) { }

        public async Task<int> Save(HealthHistory healthHistory)
        {
            return await WithConnection(async c =>
            {
                string sql = $@" INSERT INTO `health_history`(
                    
                    eqp,
                    type,
                    eqp_type,
                    trouble_level,
                    correlation_id,
                    val,
                    created_time,
                    created_by,
                    created_year,
                    created_month,
                    created_day
                ) VALUES 
                (
                    @Eqp,
                    @Type,
                    @EqpType,
                    @TroubleLevel,
                    @CorrelationId,
                    @Val,
                    @CreatedTime,
                    @CreatedBy,
                    @CreatedYear,
                    @CreatedMonth,
                    @CreatedDay
                    );
                    ";
                return await c.ExecuteAsync(sql, healthHistory);
            });
        }

        public async Task<HealthHistoryView> GetPageByParm(HealthHistoryQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,u1.user_name as cname, ")
                .Append(" dt.name as troubleLevelName,et.type_name,d.name ")
                .Append(" FROM health_history a ")
                .Append(" left join dictionary_tree dt on a.trouble_level=dt.id ")
                .Append(" left join dictionary_tree d on a.type=d.id ")
                .Append(" left join equipment_type et on a.eqp_type=et.id ")
                .Append(" left join user u1 on a.created_by=u1.id ");
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" WHERE 1=1 ");
                if (parm.Eqp!=null)
                {
                    whereSql.Append(" and a.eqp = " + parm.Eqp);
                }
                if (parm.Type != null)
                {
                    whereSql.Append(" and a.type =" + parm.Type);
                }
                if (parm.StartDate != null)
                {
                    whereSql.Append(" and a.created_time >=" + parm.StartDate);
                }
                if (parm.EndDate != null)
                {
                    whereSql.Append(" and a.created_time <=" + parm.EndDate);
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                HealthHistoryView ret = new HealthHistoryView();
                ret.rows= (await c.QueryAsync<HealthHistory>(sql.ToString())).ToList();
                ret.total = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from health_config a " + whereSql.ToString());
                return ret;
            });
        }
    }
}
