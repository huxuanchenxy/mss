using Dapper;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSS.API.Model.Data;
using System.Collections.Generic;


// Coded By admin 2020/4/7 14:56:10
namespace MSS.API.Dao.Implement
{
    public interface IHealthChartSubsystemRepo<T> where T : BaseEntity
    {
        Task<HealthChartSubsystemPageView> GetPageList(HealthChartSubsystemParm param);
        Task<HealthChartSubsystem> Save(HealthChartSubsystem obj);
        Task<HealthChartSubsystem> GetByID(long id);
        Task<int> Update(HealthChartSubsystem obj);
        Task<int> Delete(string[] ids, int userID);
        Task<int> Delete2(HealthChartSubsystem obj);
        Task<List<HealthChartView>> GetChart(HealthChartSubsystemParm parm);
    }

    public class HealthChartSubsystemRepo : BaseRepo, IHealthChartSubsystemRepo<HealthChartSubsystem>
    {
        public HealthChartSubsystemRepo(DapperOptions options) : base(options) { }

        public async Task<HealthChartSubsystemPageView> GetPageList(HealthChartSubsystemParm parm)
        {
            return await WithConnection(async c =>
            {

                StringBuilder sql = new StringBuilder();
                sql.Append($@"  SELECT 
                id,
                sub_system_id,
                val_avg,
                val_max,
                val_min,
                val_middle,
                year,
                month,
                day,
                updated_time,
                updated_by,
                created_time,created_by FROM health_chart_subsystem
                 ");
                StringBuilder whereSql = new StringBuilder();
                //whereSql.Append(" WHERE ai.ProcessInstanceID = '" + parm.ProcessInstanceID + "'");

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

                var data = await c.QueryAsync<HealthChartSubsystem>(sql.ToString());
                var total = data.ToList().Count;
                sql.Append(" order by " + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                var ets = await c.QueryAsync<HealthChartSubsystem>(sql.ToString());

                HealthChartSubsystemPageView ret = new HealthChartSubsystemPageView();
                ret.rows = ets.ToList();
                ret.total = total;
                return ret;
            });
        }

        public async Task<List<HealthChartView>> GetChart(HealthChartSubsystemParm parm)
        {
            return await WithConnection(async c =>
            {

                StringBuilder sql = new StringBuilder();
                sql.Append($@"  SELECT b.ext extid,c.name,(a.val_avg) avg,MAX(a.val_max) max,MIN(a.val_min) min,a.year,a.month
                                FROM health_chart_subsystem a
                                left join dictionary_tree b on a.sub_system_id = b.id
                                left join dictionary_tree c on b.ext = c.id
                                WHERE a.updated_time >= '{parm.startTime}' AND a.updated_time < '{parm.endTime}'
                                GROUP BY b.ext,a.year,a.month
                                ORDER BY b.ext,a.year,a.month ");

                var data = await c.QueryAsync<HealthChartView>(sql.ToString());
                return data.ToList();
            });
        }


        public async Task<HealthChartSubsystem> Save(HealthChartSubsystem obj)
        {
            return await WithConnection(async c =>
            {
                string sql = $@" INSERT INTO `health_chart_subsystem`(
                    
                    sub_system_id,
                    val_avg,
                    val_max,
                    val_min,
                    val_middle,
                    year,
                    month,
                    day,
                    updated_time,
                    updated_by,
                    created_time,
                    created_by
                ) VALUES 
                (
                    @SubSystemId,
                    @ValAvg,
                    @ValMax,
                    @ValMin,
                    @ValMiddle,
                    @Year,
                    @Month,
                    @Day,
                    @UpdatedTime,
                    @UpdatedBy,
                    @CreatedTime,
                    @CreatedBy
                    );
                    ";
                sql += "SELECT LAST_INSERT_ID() ";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, obj);
                obj.Id = newid;
                return obj;
            });
        }

        public async Task<HealthChartSubsystem> GetByID(long id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<HealthChartSubsystem>(
                    "SELECT * FROM health_chart_subsystem WHERE id = @id", new { id = id });
                return result;
            });
        }

        public async Task<int> Update(HealthChartSubsystem obj)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync($@" UPDATE health_chart_subsystem set 
                    
                    sub_system_id=@SubSystemId,
                    val_avg=@ValAvg,
                    val_max=@ValMax,
                    val_min=@ValMin,
                    val_middle=@ValMiddle,
                    year=@Year,
                    month=@Month,
                    day=@Day,
                    updated_time=@UpdatedTime,
                    updated_by=@UpdatedBy,
                    created_time=@CreatedTime,
                    created_by=@CreatedBy
                 where id=@Id", obj);
                return result;
            });
        }

        public async Task<int> Delete2(HealthChartSubsystem obj)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync($@" DELETE FROM health_chart_subsystem 
                 where sub_system_id=@SubSystemId AND year=@Year AND
                    month=@Month AND
                    day=@Day ", obj);
                return result;
            });
        }

        public async Task<int> Delete(string[] ids, int userID)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" Update health_chart_subsystem set is_del=1" +
                ",updated_time=@updated_time,updated_by=@updated_by" +
                " WHERE id in @ids ", new { ids = ids, updated_time = DateTime.Now, updated_by = userID });
                return result;
            });
        }
    }
}



