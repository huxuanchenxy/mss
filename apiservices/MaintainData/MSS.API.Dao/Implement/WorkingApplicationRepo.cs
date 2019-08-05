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
                    "select wa.*,ot.name,u.user_name,ot1.name as wpname,tcb.AreaName," +
                    "u1.user_name as rname,u.mobile,e.eqp_code,e.eqp_name from working_application wa " +
                    "LEFT JOIN equipment e on e.ID=wa.eqp " +
                    "LEFT JOIN org_tree ot on ot.ID=wa.application_part " +
                    "LEFT JOIN user u on u.id=wa.applicant " +
                    "LEFT JOIN org_tree ot1 on ot1.ID=wa.working_part " +
                    "LEFT JOIN tb_config_bigarea tcb on tcb.Id=wa.registration_station " +
                    "LEFT JOIN user u1 on u1.id=wa.response_by " +
                    "WHERE wa.id = @id", new { id = id });
                return result;
            });
        }

        public async Task<List<WorkingApplicationManager>> GetByWorkingApplication(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<WorkingApplicationManager>(
                    "select wa.*,u.user_name,u.mobile from working_application_manager wa " +
                    "LEFT JOIN user u on u.id=wa.manager " +
                    "WHERE wa.working_application = @id", new { id = id });
                if (result!=null && result.Count() > 0)
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
