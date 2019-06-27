using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System.Threading.Tasks;
using Dapper;
using MSS.API.Model.DTO;
using static MSS.API.Model.Const;
using System.Linq;

namespace MSS.API.Dao.Implement
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
                //if (parm.searchGroup!=null)
                //{
                //    whereSql.Append(" and a.group_id="+ parm.searchGroup);
                //}
                //if (parm.searchParent != null)
                //{
                //    whereSql.Append(" and a.parent_menu=" + parm.searchParent);
                //}
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                mRet.data = (await c.QueryAsync<UserOperationLogView>(sql.ToString())).ToList();
                mRet.relatedData = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from user_operation_log a where 1=1 " + whereSql.ToString());
                return mRet;
            });
        }


        public async Task<int> Add(UserOperationLog action)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" insert into Action_Info " +
                    " values (0,@request_url,@action_name,@description,@action_order,@icon, " +
                    " @level,@group_id,@parent_menu, " +
                    " @created_time,@created_by,@updated_time,@updated_by) ", action);
                return result;
            });
        }

        

    }
}
