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
                var data = await c.QueryAsync<Notification>(sql);
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
