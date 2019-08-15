using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using MSS.API.Dao;
using MSS.API.Model.Data;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using Dapper.FastCrud;
using MSS.API.Model.DTO;
using MSS.API.Dao.Common;

namespace MSS.API.Dao.Implement
{
    public class WarnningRepo : BaseRepo, IWarnningRepo<EarlyWarnning>
    {
        public WarnningRepo(DapperOptions options) : base(options) { }

        public async Task<Notification> SaveNotification(Notification noti)
        {
            return await WithConnection(async c =>
            {
                string sql = "INSERT INTO notification (eqp_id, content, notification_type, notification_type_name, status, created_time, created_by, is_del)"
                            + " Values (@EqpID, @Content, @NotificationType, @NotificationTypeName, @Status, @CreatedTime, @CreatedBy, @IsDel);";
                sql += "SELECT LAST_INSERT_ID()";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, noti);
                noti.ID = newid;
                return noti;
            });
        }

        public async Task<EarlyWarnning> SaveWarnning(EarlyWarnning warn)
        {
            return await WithConnection(async c =>
            {
                string sql = "INSERT INTO early_warnning (pid, eqp_id, content, cur_value, status, created_time, created_by, is_del)"
                            + " Values (@Pid, @EqpID, @Content, @CurValue, @Status, @CreatedTime, @CreatedBy, @IsDel);";
                sql += "SELECT LAST_INSERT_ID()";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, warn);
                warn.ID = newid;
                return warn;
            });
        }

        public async Task<EarlyWarnning> UpdateWarnning(EarlyWarnning warn)
        {
            return await WithConnection(async c =>
            {
                string sql = "UPDATE early_warnning SET status = @Status,"
                            + " updated_by = @UpdatedBy, updated_time = @UpdatedTime WHERE ID = @ID;";
                await c.ExecuteAsync(sql, warn);
                return warn;
            });
        }
    }    
}
