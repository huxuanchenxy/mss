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
    public class WorkingApplicationRepo :BaseRepo, IWorkingApplicationRepo<WorkingApplication>
    {
        public WorkingApplicationRepo(DapperOptions options) :base(options)
            {
            } 

        public async Task<WorkingApplication> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<WorkingApplication>(
                    "SELECT *,dt.name,date_format(created_time,'%Y-%m-%d') as created_date FROM equipment_history eh " +
                    "left join dictionary_tree dt on dt.id=eh.type " +
                    "WHERE eqp = @id", new { id = id });
                return result;
            });
        }


    }
}         
