using Dapper;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSS.API.Model.Data;


// Coded By admin 2020/2/19 9:38:58
namespace MSS.API.Dao.Implement
{
    public interface IPidCountDetailRepo<T> where T : BaseEntity
    {
        Task<PidCountDetailPageView> GetPageList(PidCountDetailParm param);
        Task<PidCountDetail> Save(PidCountDetail obj);
        Task<PidCountDetail> GetByID(long id);
        Task<int> Update(PidCountDetail obj);
        Task<int> Delete(string[] ids, int userID);
    }

    public class PidCountDetailRepo : BaseRepo, IPidCountDetailRepo<PidCountDetail>
    {
        public PidCountDetailRepo(DapperOptions options) : base(options) { }

        public async Task<PidCountDetailPageView> GetPageList(PidCountDetailParm parm)
        {
            return await WithConnection(async c =>
            {

                StringBuilder sql = new StringBuilder();
                sql.Append($@"  SELECT 
                a.id,
                a.pid_count_id,
                a.capacity_count_old,
                a.capacity_count_new,
                a.remain_count_old,
                a.remain_count_new,
                a.used_count_old,
                a.used_count_new,
                a.remind_count_old,
                a.remind_count_new,
                a.created_time,
                a.created_by,
                a.updated_time,a.updated_by
                ,u1.user_name as created_name
                ,u2.user_name as updated_name
                FROM pid_count_detail a
                left join user u1 on a.created_by=u1.id 
                left join user u2 on a.updated_by=u2.id
                 ");
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" WHERE a.pid_count_id = '" + parm.PidCountId + "'");

                //if (parm.AppName != null)
                //{
                //    whereSql.Append(" and ai.AppName like '%" + parm.AppName.Trim() + "%'");
                //}

                sql.Append(whereSql);
                //验证是否有参与到流程中
                //string sqlcheck = sql.ToString();
                //sqlcheck += ("AND ai.CreatedByUserID = '" + parm.UserID + "'");
                //var checkdata = await c.QueryFirstOrDefaultAsync<TaskViewModel>(sqlcheck);
                //if (checkdata == null)
                //{
                //    return null;
                //}

                var data = await c.QueryAsync<PidCountDetail>(sql.ToString());
                var total = data.ToList().Count;
                sql.Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                var ets = await c.QueryAsync<PidCountDetail>(sql.ToString());

                PidCountDetailPageView ret = new PidCountDetailPageView();
                ret.rows = ets.ToList();
                ret.total = total;
                return ret;
            });
        }

        public async Task<PidCountDetail> Save(PidCountDetail obj)
        {
            return await WithConnection(async c =>
            {
                string sql = $@" INSERT INTO `pid_count_detail`(
                    
                    pid_count_id,
                    capacity_count_old,
                    capacity_count_new,
                    remain_count_old,
                    remain_count_new,
                    used_count_old,
                    used_count_new,
                    remind_count_old,
                    remind_count_new,
                    created_time,
                    created_by,
                    updated_time,
                    updated_by
                ) VALUES 
                (
                    @PidCountId,
                    @CapacityCountOld,
                    @CapacityCountNew,
                    @RemainCountOld,
                    @RemainCountNew,
                    @UsedCountOld,
                    @UsedCountNew,
                    @RemindCountOld,
                    @RemindCountNew,
                    @CreatedTime,
                    @CreatedBy,
                    @UpdatedTime,
                    @UpdatedBy
                    );
                    ";
                sql += "SELECT LAST_INSERT_ID() ";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, obj);
                obj.ID = newid;
                return obj;
            });
        }

        public async Task<PidCountDetail> GetByID(long id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<PidCountDetail>(
                    "SELECT * FROM pid_count_detail WHERE id = @id", new { id = id });
                return result;
            });
        }

        public async Task<int> Update(PidCountDetail obj)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync($@" UPDATE pid_count_detail set 
                    
                    pid_count_id=@PidCountId,
                    capacity_count_old=@CapacityCountOld,
                    capacity_count_new=@CapacityCountNew,
                    remain_count_old=@RemainCountOld,
                    remain_count_new=@RemainCountNew,
                    used_count_old=@UsedCountOld,
                    used_count_new=@UsedCountNew,
                    remind_count_old=@RemindCountOld,
                    remind_count_new=@RemindCountNew,
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
                var result = await c.ExecuteAsync(" Update pid_count_detail set is_del=1" +
                ",updated_time=@updated_time,updated_by=@updated_by" +
                " WHERE id in @ids ", new { ids = ids, updated_time = DateTime.Now, updated_by = userID });
                return result;
            });
        }
    }
}



