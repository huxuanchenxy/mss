using Dapper;
using MSS.API.Model.Data;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Coded By admin 2020/2/17 14:20:41
namespace MSS.API.Dao.Implement
{
    public interface IPidCountRepo<T> where T : BaseEntity
    {
        Task<PidCountPageView> GetPageList(PidCountParm param);
        Task<PidCount> Save(PidCount obj);
        Task<PidCount> GetByID(long id);
        Task<int> Update(PidCount obj);
        Task<int> Delete(string[] ids, int userID);
    }

    public class PidCountRepo : BaseRepo, IPidCountRepo<PidCount>
    {
        public PidCountRepo(DapperOptions options) : base(options) { }

        public async Task<PidCountPageView> GetPageList(PidCountParm parm)
        {
            return await WithConnection(async c =>
            {

                StringBuilder sql = new StringBuilder();
                sql.Append($@"  SELECT 
                a.id,
                a.node_id,
                a.node_name,
                a.node_tip,
                a.capacity_count,
                a.remain_count,
                a.used_count,
                a.remind_count,
                a.created_time,
                a.created_by,
                a.updated_time,a.updated_by 
                ,u1.user_name as created_name
                ,u2.user_name as updated_name
                FROM pid_count a
                left join user u1 on a.created_by=u1.id 
                left join user u2 on a.updated_by=u2.id 
                 ");
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" WHERE 1 = 1 ");

                if (!string.IsNullOrEmpty(parm.nodeId))
                {
                    whereSql.Append(" and a.node_id like '%" + parm.nodeId.Trim() + "%'");
                }
                if (!string.IsNullOrEmpty(parm.nodeName))
                {
                    whereSql.Append(" and a.node_name like '%" + parm.nodeName.Trim() + "%'");
                }
                if (!string.IsNullOrEmpty(parm.nodeTip))
                {
                    whereSql.Append(" and a.node_tip like '%" + parm.nodeTip.Trim() + "%'");
                }

                sql.Append(whereSql);
                //验证是否有参与到流程中
                //string sqlcheck = sql.ToString();
                //sqlcheck += ("AND ai.CreatedByUserID = '" + parm.UserID + "'");
                //var checkdata = await c.QueryFirstOrDefaultAsync<TaskViewModel>(sqlcheck);
                //if (checkdata == null)
                //{
                //    return null;
                //}

                var data = await c.QueryAsync<PidCount>(sql.ToString());
                var total = data.ToList().Count;
                sql.Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                var ets = await c.QueryAsync<PidCount>(sql.ToString());

                PidCountPageView ret = new PidCountPageView();
                ret.rows = ets.ToList();
                ret.total = total;
                return ret;
            });
        }

        public async Task<PidCount> Save(PidCount obj)
        {
            return await WithConnection(async c =>
            {
                string sql = $@" INSERT INTO `pid_count`(
                    
                    node_id,
                    node_name,
                    node_tip,
                    capacity_count,
                    remain_count,
                    used_count,
                    remind_count,
                    created_time,
                    created_by,
                    updated_time,
                    updated_by
                ) VALUES 
                (
                    @NodeId,
                    @NodeName,
                    @NodeTip,
                    @CapacityCount,
                    @RemainCount,
                    @UsedCount,
                    @RemindCount,
                    @CreatedTime,
                    @CreatedBy,
                    @UpdatedTime,
                    @UpdatedBy
                    );
                    ";
                sql += "SELECT LAST_INSERT_ID() ";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, obj);
                obj.Id = newid;
                return obj;
            });
        }

        public async Task<PidCount> GetByID(long id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<PidCount>(
                    "SELECT * FROM pid_count WHERE id = @id", new { id = id });
                return result;
            });
        }

        public async Task<int> Update(PidCount obj)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync($@" UPDATE pid_count set 
                    
                    node_id=@NodeId,
                    node_name=@NodeName,
                    node_tip=@NodeTip,
                    capacity_count=@CapacityCount,
                    remain_count=@RemainCount,
                    used_count=@UsedCount,
                    remind_count=@RemindCount,
                    created_time=@CreatedTime,
                    created_by=@CreatedBy,
                    updated_time=@UpdatedTime,
                    updated_by=@UpdatedBy
                 where id=@Id", obj);
                return result;
            });
        }

        public async Task<int> Delete(string[] ids, int userID)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" Update pid_count set is_del=1" +
                ",updated_time=@updated_time,updated_by=@updated_by" +
                " WHERE id in @ids ", new { ids = ids, updated_time = DateTime.Now, updated_by = userID });
                return result;
            });
        }
    }
}



