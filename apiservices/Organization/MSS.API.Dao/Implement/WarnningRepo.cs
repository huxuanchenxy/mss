﻿using System;
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

        public async Task<Notification> UpdateNotification(Notification noti)
        {
            return await WithConnection(async c =>
            {
                string sql = "UPDATE notification SET status = @Status,"
                            + " updated_by = @UpdatedBy, updated_time = @UpdatedTime WHERE ID = @ID;";
                await c.ExecuteAsync(sql, noti);
                return noti;
            });
        }

        public async Task<Notification> GetNotificationByID(int notificationID)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * FROM notification WHERE ID = @ID;";
                Notification data = await c.QueryFirstOrDefaultAsync<Notification>(sql,
                new
                {
                    ID = notificationID
                });
                return data;
            });
        }

        public async  Task<List<EarlyWarnning>> ListAllEarlyWarnning()
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT a.*, b.eqp_code, b.eqp_name, b.eqp_type, c.type_name"
                    + " FROM early_warnning a"
                    + " JOIN equipment b ON a.eqp_id=b.id AND a.status=0 AND a.is_del != 1"
                    + " JOIN equipment_type c on c.id=b.eqp_type";
                var data = await c.QueryAsync<EarlyWarnning>(sql);
                return data.ToList();
            });
        }

        public async Task<List<EarlyWarnning>> ListEarlyWarnningByOrg(int? orgID)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT a.*, b.eqp_code, b.eqp_name, b.eqp_type, c.type_name"
                    + " FROM early_warnning a"
                    + " JOIN equipment b ON a.eqp_id=b.id AND a.status=0 AND a.is_del != 1"
                    + " JOIN equipment_type c on c.id=b.eqp_type";
                if (orgID != null)
                {
                    sql += " WHERE b.top_org = " + orgID;
                }
                sql += " order by a.created_time desc";
                var data = await c.QueryAsync<EarlyWarnning>(sql);
                return data.ToList();
            });
        }

        public async Task<List<Notification>> ListNotificationByOrg(int? orgID)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT a.*, b.eqp_code, b.eqp_name, b.eqp_type, c.type_name"
                    + " FROM notification a"
                    + " JOIN equipment b ON a.eqp_id=b.id AND a.status=0 AND a.is_del != 1"
                    + " JOIN equipment_type c on c.id=b.eqp_type";
                if (orgID != null)
                {
                    sql += " WHERE b.top_org = " + orgID;
                }
                sql += " order by a.created_time desc";
                var data = await c.QueryAsync<Notification>(sql);
                return data.ToList();
            });
        }

        public async Task<List<PidTable>> ListPidTableByOrg(int? orgID, int? eqpType)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT a.*, b.eqp_code, b.eqp_name, b.eqp_type, c.type_name, b.top_org"
                    + " FROM pid_table a"
                    + " JOIN equipment b ON a.eqp_id=b.id"
                    + " JOIN equipment_type c ON c.id=b.eqp_type WHERE 1=1";
                if (orgID != null)
                {
                    sql += " AND b.top_org = " + orgID;
                }
                if (eqpType != null)
                {
                    sql += " AND b.eqp_type = " + eqpType;
                }
                var data = await c.QueryAsync<PidTable>(sql);
                return data.ToList();
            });
        }

        public async Task<PageData<EarlyWarnning>> ListEarlyWarningHistory(WarningParam param)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                StringBuilder sqlCount = new StringBuilder();

                sql.Append("SELECT a.*, b.eqp_code, b.eqp_name, b.eqp_type, c.type_name");
                sqlCount.Append("SELECT COUNT(*)");

                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" FROM early_warnning a");
                whereSql.Append(" JOIN equipment b ON a.eqp_id=b.id AND a.status=0 AND a.is_del != 1");
                whereSql.Append(" JOIN equipment_type c on c.id=b.eqp_type where 1=1");
                
                if (param.orgID != null)
                {
                    whereSql.Append(" AND b.top_org = " + param.orgID);
                }
                if (param.eqpTypeID != null)
                {
                    whereSql.Append(" AND b.eqp_type = " + param.eqpTypeID);
                }
                if (param.startTime != null)
                {
                    whereSql.Append(" AND a.created_time >= '" + param.startTime + "'");
                }
                if (param.endTime != null)
                {
                    whereSql.Append(" AND a.created_time <= '" + param.endTime + "'");
                }
                if (param.status != null)
                {
                    whereSql.Append(" AND a.status = " + param.status);
                }
                sql.Append(whereSql)
                   .Append(" order by a." + param.sort + " " + param.order)
                   .Append(" limit " + (param.page - 1) * param.rows + "," + param.rows);
                sqlCount.Append(whereSql);
                var data = await c.QueryAsync<EarlyWarnning>(sql.ToString());
                int total = await c.QueryFirstOrDefaultAsync<int>(sqlCount.ToString());

                PageData<EarlyWarnning> ret = new PageData<EarlyWarnning>();
                ret.rows = data.ToList();
                ret.total = total;

                return ret;
            });
        }

        public async Task<PageData<Notification>> ListNotificationHistory(NotificationParam param)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                StringBuilder sqlCount = new StringBuilder();

                sql.Append("SELECT a.*, b.eqp_code, b.eqp_name, b.eqp_type, c.type_name");
                sqlCount.Append("SELECT COUNT(*)");

                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" FROM notification a");
                whereSql.Append(" JOIN equipment b ON a.eqp_id=b.id AND a.is_del != 1");
                whereSql.Append(" JOIN equipment_type c on c.id=b.eqp_type where 1=1");

                if (param.orgID != null)
                {
                    whereSql.Append(" AND b.top_org = " + param.orgID);
                }
                if (param.eqpTypeID != null)
                {
                    whereSql.Append(" AND b.eqp_type = " + param.eqpTypeID);
                }
                if (param.startTime != null)
                {
                    whereSql.Append(" AND a.created_time >= '" + param.startTime + "'");
                }
                if (param.endTime != null)
                {
                    whereSql.Append(" AND a.created_time <= '" + param.endTime + "'");
                }
                if (param.status != null)
                {
                    whereSql.Append(" AND a.status = " + param.status);
                }
                if (param.type != null)
                {
                    whereSql.Append(" AND a.notification_type = " + param.type);
                }
                sql.Append(whereSql)
                   .Append(" order by a." + param.sort + " " + param.order)
                   .Append(" limit " + (param.page - 1) * param.rows + "," + param.rows);
                sqlCount.Append(whereSql);
                var data = await c.QueryAsync<Notification>(sql.ToString());
                int total = await c.QueryFirstOrDefaultAsync<int>(sqlCount.ToString());

                PageData<Notification> ret = new PageData<Notification>();
                ret.rows = data.ToList();
                ret.total = total;

                return ret;
            });
        }
    }    
}
