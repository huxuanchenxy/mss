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
    public interface IHealthConfigRepo<T>
    {
        Task<int> Save(HealthConfig hConfig);
        Task<int> Save(List<HealthConfig> hConfigs);
        Task<int> Update(HealthConfig hConfig);
        Task<int> Update(List<HealthConfig> hConfigs);
        Task<int> Delete(int type,int? eqpType=null);
        Task<HealthConfigView> GetPageByParm(HealthConfigQueryParm parm);
        Task<HealthConfig> GetByType(int type);
        Task<List<HealthConfig>> GetByTroubleLevel(int type, int eqpType);

        Task<List<HealthConfig>> GetValByCon(int type, List<int> eqpType = null, int? troubleLevel = null);
        Task<List<Equipment>> GetEqpTypeByEqps(List<int> eqps);
    }

    public class HealthConfigRepo : BaseRepo, IHealthConfigRepo<HealthConfig>
    {
        public HealthConfigRepo(DapperOptions options) : base(options) { }

        public async Task<int> Save(HealthConfig hConfig)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into health_config " +
                        " values (0,@Type,@EqpType,@TroubleLevel,@Val," +
                        "@CreatedTime,@CreatedBy,@UpdatedTime,@UpdatedBy); ";
                return await c.ExecuteAsync(sql, hConfig);
            });
        }
        public async Task<int> Save(List<HealthConfig> hConfigs)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into health_config " +
                        " values (0,@Type,@EqpType,@TroubleLevel,@Val," +
                        "@CreatedTime,@CreatedBy,@UpdatedTime,@UpdatedBy); ";
                return await c.ExecuteAsync(sql, hConfigs);
            });
        }

        public async Task<int> Update(HealthConfig hConfig)
        {
            return await WithConnection(async c =>
            {
                string sql = " update health_config " +
                        " set type=@Type,eqp_type=@EqpType,trouble_level=@TroubleLevel," +
                        " val=@Val,updated_by=@UpdatedBy,updated_time=@UpdatedTime where id=@ID";
                return await c.ExecuteAsync(sql, hConfig);
            });
        }
        public async Task<int> Update(List<HealthConfig> hConfigs)
        {
            return await WithConnection(async c =>
            {
                string sql = " update health_config " +
                        " set type=@Type,eqp_type=@EqpType,trouble_level=@TroubleLevel," +
                        " val=@Val,updated_by=@UpdatedBy,updated_time=@UpdatedTime where id=@ID";
                return await c.ExecuteAsync(sql, hConfigs);
            });
        }

        public async Task<int> Delete(int type, int? eqpType=null)
        {
            return await WithConnection(async c =>
            {
                string sql= " Delete from health_config"+
                " WHERE type=" + type;
                if (eqpType!=null)
                {
                    sql += " and eqp_type=" + eqpType;
                }
                var result = await c.ExecuteAsync(sql);
                return result;
            });
        }

        public async Task<HealthConfigView> GetPageByParm(HealthConfigQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,u1.user_name as cname,u2.user_name, ")
                .Append(" dt.name as troubleLevelName,et.type_name,d.name ")
                .Append(" FROM health_config a ")
                .Append(" left join dictionary_tree dt on a.trouble_level=dt.id ")
                .Append(" left join dictionary_tree d on a.type=d.id ")
                .Append(" left join equipment_type et on a.eqp_type=et.id ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.updated_by=u2.id ");
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" WHERE 1=1 ");
                if (parm.EqpType!=null)
                {
                    whereSql.Append(" and a.eqp_type = " + parm.EqpType);
                }
                if (parm.Type != null)
                {
                    whereSql.Append(" and a.type =" + parm.Type);
                }
                if (parm.TroubleLevel!=null)
                {
                    whereSql.Append(" and a.trouble_level ="+parm.TroubleLevel);
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                HealthConfigView ret = new HealthConfigView();
                ret.rows= (await c.QueryAsync<HealthConfig>(sql.ToString())).ToList();
                ret.total = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from health_config a " + whereSql.ToString());
                return ret;
            });
        }
        /// <summary>
        /// type-value
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<HealthConfig> GetByType(int type)
        {
            return await WithConnection(async c =>
            {
                HealthConfig result = new HealthConfig();
                string sql = "SELECT hc.ID,dt.name,dt.des,hc.val" +
                " from dictionary_tree dt "+
                " left join health_config hc on hc.type=dt.ID " +
                " where dt.id=" +type;
                result = await c.QueryFirstOrDefaultAsync<HealthConfig>(sql);
                return result;
            });
        }

        /// <summary>
        /// type,eqpType,troubleLevel-value
        /// </summary>
        /// <param name="type"></param>
        /// <param name="eqpType"></param>
        /// <returns></returns>
        public async Task<List<HealthConfig>> GetByTroubleLevel(int type, int eqpType)
        {
            return await WithConnection(async c =>
            {
                List<HealthConfig> result = new List<HealthConfig>();
                string sql = "SELECT hc.ID,dt.name,dt.des,hc.val,hc.eqp_type," +
                " d.id as trouble_level,d.name as TroubleLevelName " +
                " from dictionary_tree d " +
                " left join health_config hc on hc.trouble_level=d.ID and hc.eqp_type=" + eqpType +
                " left join dictionary_tree dt on hc.type=dt.ID and dt.id=" + type +
                " where d.parent_id=" + Dictionary.TROUBLELEVEL;
                result = (await c.QueryAsync<HealthConfig>(sql)).ToList();
                return result;
            });
        }

        public async Task<List<HealthConfig>> GetValByCon(int type, List<int> eqpType=null,int? troubleLevel=null)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * from health_config  " +
                " where type=" + type;
                if (eqpType!=null)
                {
                    sql += " and eqp_type in (" + string.Join(",",eqpType)+") ";
                }
                if (troubleLevel!=null)
                {
                    sql += " and trouble_level=" + troubleLevel;
                }
                var tmp = await c.QueryAsync<HealthConfig>(sql);
                if (tmp.Count()>0)
                {
                    return tmp.ToList();
                }
                else
                {
                    return new List<HealthConfig>();
                }
            });
        }

        public async Task<List<Equipment>> GetEqpTypeByEqps(List<int> eqps)
        {
            return await WithConnection(async c =>
            {
                List<Equipment> result = new List<Equipment>();
                string sql = "SELECT e.id,e.eqp_type from equipment e " +
                " where e.id in @eqps";
                result = (await c.QueryAsync<Equipment>(sql,new {eqps })).ToList();
                return result;
            });
        }

    }
}
