using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using MSS.API.Dao;
using MSS.API.Model.Data;
using System.Threading.Tasks;
using Dapper;
using MSS.API.Model.DTO;
using static MSS.API.Common.Const;
using System.Linq;

namespace MSS.API.Dao.Implement
{
    public class ActionGroupRepo : BaseRepo, IActionGroupRepo<ActionGroup>
    {
        public ActionGroupRepo(DapperOptions options) : base(options) { }

        public async Task<MSSResult<ActionGroupView>> GetPageByParm(ActionGroupQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                MSSResult<ActionGroupView> mRet = new MSSResult<ActionGroupView>();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,u1.user_name as created_name,u2.user_name as updated_name,d.sub_code_name as group_type_name ")
                .Append(" FROM (Action_Group a,dictionary d) ")
                .Append(" left join user u1 on a.created_by=u1.id left join user u2 on a.updated_by=u2.id ")
                .Append(" WHERE d.code='" + STR_GROUP_TYPE + "' and d.sub_code=a.group_type ");
                StringBuilder whereSql = new StringBuilder();
                if (!string.IsNullOrWhiteSpace(parm.searchName))
                {
                    whereSql.Append(" and a.group_name like '%"+ parm.searchName + "%' ");
                }
                if (parm.searchType!=null)
                {
                    whereSql.Append(" and a.group_type="+ parm.searchType);
                }
                sql.Append(whereSql)
                .Append("order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                mRet.data = (await c.QueryAsync<ActionGroupView>(sql.ToString())).ToList();
                mRet.relatedData = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from Action_Group where 1=1 "+whereSql.ToString());
                return mRet;
            });
        }
        public async Task<ActionGroup> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<ActionGroup>(
                    "SELECT * FROM Action_Group WHERE id = @id", new { id = id });
                return result;
            });
        }

        public async Task<int> Add(ActionGroup actionGroup)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" insert into Action_Group " +
                    " values (0,@group_name,@request_url,@group_type,@group_order,@icon, " +
                    " @created_time,@created_by,@updated_time,@updated_by) ", actionGroup);
                return result;
            });
        }

        public async Task<int> Update(ActionGroup actionGroup)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" update Action_Group " +
                    " set group_name=@group_name,request_url=@request_url,group_type=@group_type,group_order=@group_order,icon=@icon, " +
                    " updated_time=@updated_time,updated_by=@updated_by) where id=@id", actionGroup);
                return result;
            });
        }

        public async Task<int> Delete(string[] ids)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" Delete from Action_Group WHERE id in @ids ", new { ids = ids });
                return result;
            });
        }

        public async Task<List<ActionGroup>> GetAll()
        {
            return await WithConnection(async c =>
            {
                var result = (await c.QueryAsync<ActionGroup>("select * from action_group")).ToList();
                return result;
            });
        }
    }
}
