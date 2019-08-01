using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using System.Threading.Tasks;
using Dapper;
using System.Linq;
using Dapper.FastCrud;

namespace MSS.API.Dao.Implement
{
    public class EqpHistoryRepo :BaseRepo, IEqpHistoryRepo<EqpHistory>
    {
        public EqpHistoryRepo(DapperOptions options) :base(options)
            {
            } 

        public async Task<List<EqpHistory>> ListByEqp(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<EqpHistory>(
                    "SELECT *,dt.name,date_format(created_time,'%Y-%m-%d') as created_date FROM equipment_history eh " +
                    "left join dictionary_tree dt on dt.id=eh.type " +
                    "WHERE eqp = @id", new { id = id });
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

    }
}         
