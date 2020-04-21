using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using Dapper.FastCrud;
using static MSS.API.Common.MyDictionary;

namespace MSS.API.Dao.Implement
{
    public class EqpHistoryRepo :BaseRepo, IEqpHistoryRepo<EqpHistory>
    {
        public EqpHistoryRepo(DapperOptions options) :base(options)
            {
            } 

        public async Task<List<EqpHistory>> ListByEqp(int id,bool isHide)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT *,dt.name,date_format(created_time,'%Y-%m-%d') as created_date FROM equipment_history eh " +
                    "left join dictionary_tree dt on dt.id=eh.type " +
                    "WHERE eqp = @id ";
                if (isHide)
                {
                    sql += " and type!=" + (int)EqpHistoryType.Maintenance;
                }
                sql += " order by created_time";
                var result = await c.QueryAsync<EqpHistory>(sql,new { id = id });
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                else
                {
                    return null;
                }
            });
        }

        public async Task<List<EqpHistory>> ListByType(string[] types)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<EqpHistory>(
                    "select * from equipment_history where id in(" +
                    "select Max(id) from equipment_history " +
                    "WHERE type in @types " +
                    "GROUP BY type,eqp) ", new { types });
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                else
                {
                    return null;
                }
            });
        }

        public async Task<List<QueryItem>> ListAllLocations()
        {
            return await WithConnection(async c =>
            {
                string sql = "select AreaName as name,id,1 as LocationBy from tb_config_bigarea UNION " +
                "select AreaName as name,id,2 as LocationBy from tb_config_midarea";
                var result = await c.QueryAsync<QueryItem>(sql);
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                else
                {
                    return null;
                }
            });
        }
        public async Task<int> SaveEqpHistory(List<EqpHistory> eqps)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into equipment_history " +
                        " values (0,@EqpID,@Type,@WorkingOrder,@ShowName,@CreatedTime,@CreatedBy); ";
                return await c.ExecuteAsync(sql, eqps);
            });
        }
        public async Task<int> SaveEqpHistory(EqpHistory eh)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into equipment_history " +
                        " values (0,@EqpID,@Type,@WorkingOrder,@ShowName,@CreatedTime,@CreatedBy); ";
                return await c.ExecuteAsync(sql, eh);
            });
        }

        public async Task<DateTime> OnLineByEqp(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<Equipment>(
                    "select online_date from equipment where id=@id", new { id });
                return result.FirstOrDefault().Online;
            });
        }

    }
}         
