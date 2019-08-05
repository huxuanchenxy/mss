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
    public class TroubleReportRepo :BaseRepo, ITroubleReportRepo<TroubleReport>
    {
        public TroubleReportRepo(DapperOptions options) :base(options)
            {
            } 

        public async Task<TroubleReport> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<TroubleReport>(
                    "select tr.*,e.eqp_name,e.eqp_code,dt.`name` as typeName,dt1.name," +
                    "u.user_name,u1.user_name as cname,u2.user_name as aname " +
                    "from trouble_report tr " +
                    "LEFT JOIN equipment e on e.id=tr.eqp " +
                    "LEFT JOIN dictionary_tree dt on dt.ID=tr.type " +
                    "LEFT JOIN dictionary_tree dt1 on dt1.ID=tr.`status` " +
                    "LEFT JOIN `user` u on u.id=tr.reported_by " +
                    "LEFT JOIN `user` u1 on u1.id=tr.created_by " +
                    "LEFT JOIN `user` u2 on u2.id=tr.accepted_by " +
                    "WHERE tr.id = @id", new { id = id });
                return result;
            });
        }
    }
}         
