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

namespace MSS.API.Dao.Implement
{
    public class WarnningSettingRepo : BaseRepo, IWarnningSettingRepo<EarlyWarnningSetting>
    {
        public WarnningSettingRepo(DapperOptions options) : base(options) { }

        public async Task<List<EarlyWarnningSetting>> ListAllWarnningSetting()
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,d.type_name as eqp_type_name,e.user_name,c.pid_name,b.param_id, c.pid, b.param_limit_type,b.param_limit_value FROM early_warnning_setting AS a");
                sql.Append(" LEFT JOIN early_warnning_setting_ex AS b ON a.ID=b.early_warnning_id AND b.is_del!=1");
                sql.Append(" LEFT JOIN early_warnning_ex_type AS c on b.param_id=c.ID");
                sql.Append(" JOIN equipment_type AS d on a.equipment_type_id=d.ID");
                sql.Append(" JOIN user AS e on a.updated_by=e.ID");
                sql.Append(" WHERE a.is_del!=1");

                var orderDictionary = new Dictionary<int, EarlyWarnningSetting>();
                var list = await c.QueryAsync<EarlyWarnningSetting, EarlyWarnningSettingEx, EarlyWarnningSetting>(
                    sql.ToString(),
                    (setting, settingEx) =>
                    {
                        EarlyWarnningSetting settingEntity;

                        if (!orderDictionary.TryGetValue(setting.ID, out settingEntity))
                        {
                            settingEntity = setting;
                            settingEntity.SettingEx = new List<EarlyWarnningSettingEx>();
                            orderDictionary.Add(settingEntity.ID, settingEntity);
                        }
                        if (settingEx != null)
                        {
                            settingEntity.SettingEx.Add(settingEx);
                        }
                        return settingEntity;
                    },
                    splitOn: "pid_name");
                return list.Distinct().ToList();
            });
        }

        // 获取所有设备信息
        public async Task<List<Equipment>> ListAllEquipment()
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT id, eqp_code, eqp_name, eqp_type, top_org, team, online_date,"
                    + " online_again, life, medium_repair, large_repair from equipment"
                    + " WHERE is_del != 1";
                List<Equipment> eqps = (await c.QueryAsync<Equipment>(sql)).ToList();
                return eqps;
            });
        }

        public async Task<List<PidTable>> ListAllPid()
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT a.*, b.eqp_type FROM pid_table a"
                    + " JOIN equipment b on a.eqp_id=b.id";
                var list = await c.QueryAsync<PidTable>(sql);

                return list.ToList();
            });
        }

        // 插入pid表
        public async Task<int> SavePidTable(List<PidTable> data)
        {
            return await WithConnection(async c =>
            {
                string sql = "INSERT INTO pid_table (PID, eqp_id, prop, Des, pid_type, UT, UP, DW, UUP, DDW)"
                            + " Values (@pid, @EqpID, @prop, @Des, @PidType, @UT, @UP, @DW, @UUP, @DDW);";
                int affectedRows = await c.ExecuteAsync(sql, data);
                return affectedRows;
            });
        }

        // 删除指定pid
        public async Task<int> DeletePidTable(List<PidTable> data)
        {
            return await WithConnection(async c =>
            {
                string sql = "DELETE FROM pid_table WHERE PID = @pid";
                int affectedRows = await c.ExecuteAsync(sql, data);
                return affectedRows;
            });
        }
    }    
}
