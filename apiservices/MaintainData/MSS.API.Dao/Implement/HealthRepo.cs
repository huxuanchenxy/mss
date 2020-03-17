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
    public interface IHealthRepo<T>
    {
        Task<int> Save(Health health);
        Task<int> Update(Health health);
        Task<HealthView> GetPageByParm(HealthQueryParm parm);

        Task<List<Health>> ListEqp(List<int> eqps);
    }

    public class HealthRepo : BaseRepo, IHealthRepo<Health>
    {
        public HealthRepo(DapperOptions options) : base(options) { }

        public async Task<int> Save(Health health)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into health " +
                        " values (0,@Eqp,@Type,@EqpType,@TroubleLevel,@CorrelationID," +
                        "@Val,@CreatedTime,@CreatedBy,@UpdatedTime,@UpdatedBy); ";
                return await c.ExecuteAsync(sql, health);
            });
        }

        public async Task<int> Update(Health health)
        {
            return await WithConnection(async c =>
            {
                string sql = " update health " +
                        " set eqp=@Eqp,type=@Type,eqp_type=@EqpType," +
                        "trouble_level=@TroubleLevel,correlation_id=@CorrelationID," +
                        " val=@Val,updated_by=@UpdatedBy,updated_time=@UpdatedTime where id=@ID";
                return await c.ExecuteAsync(sql, health);
            });
        }

        public async Task<HealthView> GetPageByParm(HealthQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,u1.user_name as cname,u2.user_name, ")
                .Append(" dt.name as troubleLevelName,et.type_name,d.name ")
                .Append(" FROM health a ")
                .Append(" left join dictionary_tree dt on a.trouble_level=dt.id ")
                .Append(" left join dictionary_tree d on a.type=d.id ")
                .Append(" left join equipment_type et on a.eqp_type=et.id ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.updated_by=u2.id ");
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
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                HealthView ret = new HealthView();
                ret.rows= (await c.QueryAsync<Health>(sql.ToString())).ToList();
                ret.total = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from health_config a " + whereSql.ToString());
                return ret;
            });
        }
        public async Task<List<Health>> ListEqp(List<int> eqps)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT * from health where eqp in @eqps ");
                var tmp=await c.QueryAsync<Health>(sql.ToString(), new { eqps });
                if (tmp.Count() > 0)
                {
                    return tmp.ToList();
                }
                else
                {
                    return new List<Health>();
                }
            });
        }
    }
}
