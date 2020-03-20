using Dapper;
using MSS.API.Model.Data;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Coded By admin 2020/2/20 14:50:00
namespace MSS.API.Dao.Implement
{
    public interface INotificationPidcountRepo<T> where T : BaseEntity
    {
        Task<NotificationPidcountPageView> GetPageList(NotificationPidcountParm param);
        Task<NotificationPidcount> Save(NotificationPidcount obj);
        Task<int> UpdateOtherPidCount(NotificationPidcount obj);
        Task<NotificationPidcount> GetByID(long id);
        Task<int> Update(NotificationPidcount obj);
        Task<int> Delete(string[] ids, int userID);
    }

    public class NotificationPidcountRepo : BaseRepo, INotificationPidcountRepo<NotificationPidcount>
    {
        public NotificationPidcountRepo(DapperOptions options) : base(options) { }

        public async Task<NotificationPidcountPageView> GetPageList(NotificationPidcountParm parm)
        {
            return await WithConnection(async c =>
            {

                StringBuilder sql = new StringBuilder();
                sql.Append($@"  SELECT 
                ID,
                pid_count_id,
                pid_count_name,
                content,
                status,
                created_by,
                created_time,
                updated_by,
                updated_time,is_del FROM notification_pidcount
                 ");
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" WHERE is_del = 0 ");

                if (parm.status != -1)
                {
                    whereSql.Append(" and status = '" + parm.status + "'");
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

                var data = await c.QueryAsync<NotificationPidcount>(sql.ToString());
                var total = data.ToList().Count;
                sql.Append(" order by " + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                var ets = await c.QueryAsync<NotificationPidcount>(sql.ToString());

                NotificationPidcountPageView ret = new NotificationPidcountPageView();
                ret.rows = ets.ToList();
                ret.total = total;
                return ret;
            });
        }

        public async Task<NotificationPidcount> Save(NotificationPidcount obj)
        {
            return await WithConnection(async c =>
            {
                string sql = $@" INSERT INTO `notification_pidcount`(
                    ID,
                    pid_count_id,
                    pid_count_name,
                    content,
                    status,
                    created_by,
                    created_time,
                    updated_by,
                    updated_time,
                    is_del
                ) VALUES 
                (@ID,
                    @PidCountId,
                    @PidCountName,
                    @Content,
                    @Status,
                    @CreatedBy,
                    @CreatedTime,
                    @UpdatedBy,
                    @UpdatedTime,
                    @IsDel
                    );
                    ";
                sql += "SELECT LAST_INSERT_ID() ";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, obj);
                obj.ID = newid;
                return obj;
            });
        }

        public async Task<NotificationPidcount> GetByID(long id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<NotificationPidcount>(
                    "SELECT * FROM notification_pidcount WHERE id = @id", new { id = id });
                return result;
            });
        }

        public async Task<int> Update(NotificationPidcount obj)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync($@" UPDATE notification_pidcount set 
                    ID=@ID,
                    pid_count_id=@PidCountId,
                    pid_count_name=@PidCountName,
                    content=@Content,
                    status=@Status,
                    updated_by=@UpdatedBy,
                    updated_time=@UpdatedTime,
                    is_del=@IsDel
                 where id=@Id", obj);
                return result;
            });
        }

        public async Task<int> UpdateOtherPidCount(NotificationPidcount obj)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync($@" UPDATE notification_pidcount set 
                    status=@Status,
                    updated_by=@UpdatedBy,
                    updated_time=@UpdatedTime
                 where pid_count_id=@PidCountId", obj);
                return result;
            });
        }

        public async Task<int> Delete(string[] ids, int userID)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" Update notification_pidcount set is_del=1" +
                ",updated_time=@updated_time,updated_by=@updated_by" +
                " WHERE id in @ids ", new { ids = ids, updated_time = DateTime.Now, updated_by = userID });
                return result;
            });
        }
    }
}



