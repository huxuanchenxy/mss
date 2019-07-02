using System.Collections.Generic;
using System.Text;
using MSS.API.Operlog.Dao.Interface;
using MSS.API.Operlog.Model.Data;
using System.Threading.Tasks;
using Dapper;
using MSS.API.Operlog.Model.DTO;
using static MSS.API.Operlog.Model.Const;
using System.Linq;

namespace MSS.API.Operlog.Dao.Implement
{
    public class UserOperationLogRepo : BaseRepo, IUserOperationLogRepo<UserOperationLog>
    {
        public UserOperationLogRepo(DapperOptions options) : base(options) { }

        public async Task<MSSResult<UserOperationLogView>> GetPageByParm(UserOperationLogParm parm)
        {
            return await WithConnection(async c =>
            {
                MSSResult<UserOperationLogView> mRet = new MSSResult<UserOperationLogView>();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.* FROM user_operation_log a  ")
                .Append(" where 1=1 ");
                StringBuilder whereSql = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(parm.actionName))
                {
                    whereSql.Append(" and a.action_name like '%" + parm.actionName + "%' ");
                }
                if (!string.IsNullOrWhiteSpace(parm.methodName))
                {
                    whereSql.Append(" and a.method_name like '%" + parm.methodName + "%' ");
                }
                if (!string.IsNullOrWhiteSpace(parm.userName))
                {
                    whereSql.Append(" and a.user_name like '%" + parm.userName + "%' ");
                }
                if (!string.IsNullOrWhiteSpace(parm.startTime) && !string.IsNullOrWhiteSpace(parm.endTime))
                {
                    whereSql.Append(" and ( a.created_time >= '" + parm.startTime + "' AND a.created_time <= '" + parm.endTime + "' ) ");
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                mRet.data = (await c.QueryAsync<UserOperationLogView>(sql.ToString())).ToList();
                mRet.relatedData = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from user_operation_log a where 1=1 " + whereSql.ToString());
                return mRet;
            });
        }


        public async Task<int> Add(UserOperationLog obj)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" insert into user_operation_log " +
                    "( `controller_name`, `action_name`, `method_name`, `acc_name`, `user_name`, `ip`, `mac_add`, `created_by`, `updated_time`, `updated_by`, `is_del`) " +
                    " values (@controller_name,@action_name,@method_name,@acc_name,@user_name, " +
                    " @ip,@mac_add, " +
                    " @created_by,@updated_time,@updated_by,@is_del) ", obj);
                return result;
            });
        }

        

    }
}
