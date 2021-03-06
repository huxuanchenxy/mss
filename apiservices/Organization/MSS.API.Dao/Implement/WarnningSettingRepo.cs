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

namespace MSS.API.Dao.Implement
{
    public class WarnningSettingRepo : BaseRepo, IWarnningSettingRepo<EarlyWarnningSetting>
    {
        public WarnningSettingRepo(DapperOptions options) : base(options) { }

        public async Task<EarlyWarnningSetting> SaveWarnningSetting(EarlyWarnningSetting setting)
        {
            return await WithConnection(async c =>
            {
                string sql = "INSERT INTO early_warnning_setting (equipment_type_id, param_id, param_name, param_unit, param_limit_upper, param_limit_lower, is_actived"
                            + ",expert, created_time, created_by, updated_by, updated_time, is_del)"
                            + " Values (@EquipmentTypeID, @ParamID, @ParamName, @ParamUnit, @ParamLimitUpper, @ParamLimitLower, @IsActived,@Expert, @CreatedTime, @CreatedBy, @UpdatedBy, @UpdatedTime, @IsDel);";
                sql += "SELECT LAST_INSERT_ID()";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql,
                    setting);
                setting.ID = newid;
                return setting;
            });
        }

        public async Task<bool> CheckWarnExist(EarlyWarnningSetting setting)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * FROM early_warnning_setting WHERE equipment_type_id=@EquipmentTypeID and param_id=@ParamID and is_del!=1";
                EarlyWarnningSetting exist = await c.QueryFirstOrDefaultAsync<EarlyWarnningSetting>(sql,
                new
                {
                    EquipmentTypeID = setting.EquipmentTypeID,
                    ParamID = setting.ParamID
                });
                return exist != null ? true : false;
            });
        }

        public async Task<int> SaveWarnningSettingEx(List<EarlyWarnningSettingEx> settingEx)
        {
            return await WithConnection(async c =>
            {
                string sql = "INSERT INTO early_warnning_setting_ex (early_warnning_id, param_id, param_limit_type, param_limit_value"
                            +", created_time, created_by, updated_by, updated_time, is_del)"
                            + " Values (@EarlyWarnningID, @ParamID, @ParamLimitType, @ParamLimitValue, @CreatedTime, @CreatedBy, @UpdatedBy, @UpdatedTime, @IsDel);";
                int affectedRows = await c.ExecuteAsync(sql, settingEx);
                return affectedRows;
            });
        }

        public async Task<EarlyWarnningSetting> UpdateWarnningSetting(EarlyWarnningSetting setting)
        {
            return await WithConnection(async c =>
            {
                string sql = "UPDATE early_warnning_setting SET equipment_type_id =@EquipmentTypeID, param_id = @ParamID, param_name = @ParamName, param_unit = @ParamUnit, param_limit_upper = @ParamLimitUpper, param_limit_lower = @ParamLimitLower, is_actived = @IsActived,"
                            + "expert=@Expert, updated_by = @UpdatedBy, updated_time = @UpdatedTime WHERE ID=@ID;";
                await c.ExecuteAsync(sql, setting);
                return setting;
            });
        }

        public async Task<int> DeleteWarnningSettingEx(EarlyWarnningSetting setting)
        {
            return await WithConnection(async c =>
            {
                string sql = "UPDATE early_warnning_setting_ex SET is_del = 1,"
                                + " updated_by = @UpdatedBy, updated_time = @UpdatedTime WHERE early_warnning_id = @EarlyWarnningID and is_del != 1;";
                int affectedRows = await c.ExecuteAsync(sql,
                new
                {
                    EarlyWarnningID = setting.ID,
                    UpdatedBy = setting.UpdatedBy,
                    UpdatedTime = setting.UpdatedTime
                });
                return affectedRows;
            });
        }

        public async Task<bool> DeleteWarnningSetting(EarlyWarnningSetting setting)
        {
            return await WithConnection(async c =>
            {
                string sql = "UPDATE early_warnning_setting SET is_del = 1,"
                                + " updated_by = @UpdatedBy, updated_time = @UpdatedTime WHERE ID = @ID;";
                await c.ExecuteAsync(sql, setting);
                return true;
            });
        }

        public async Task<List<EarlyWarnningSetting>> ListWarnningSettingByPage(int? page, int? size, string sort, string order,
            int? eqpTypeID, string paramID)
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
                if (eqpTypeID != null)
                {
                    sql.Append(" and a.equipment_type_id=" + eqpTypeID);
                }
                if (!string.IsNullOrEmpty(paramID))
                {
                    sql.Append(" and a.param_id='" + paramID + "'");
                }
                if (!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(order))
                {
                    sql.Append(" order by a." + sort + " " + order);
                }
                if (page != null && size != null)
                {
                    sql.Append(" limit " + (page - 1) * size + "," + size);
                }
                
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

        public async Task<EarlyWarnningSetting> GetWarnningSettingByID(int id) {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,d.type_name as eqp_type_name,e.user_name,c.pid_name,b.param_id, b.param_limit_type,b.param_limit_value FROM early_warnning_setting AS a");
                sql.Append(" LEFT JOIN early_warnning_setting_ex AS b ON a.ID=b.early_warnning_id AND b.is_del!=1");
                sql.Append(" LEFT JOIN early_warnning_ex_type AS c on b.param_id=c.ID");
                sql.Append(" JOIN equipment_type AS d on a.equipment_type_id=d.ID");
                sql.Append(" JOIN user AS e on a.updated_by=e.ID");
                sql.Append(" WHERE a.is_del!=1");
                sql.Append(" and a.id=" + id);
                
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
                return list.Distinct().FirstOrDefault();
            });
        }

        public async Task<int> Count(int? eqpTypeID, string paramID)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT count(*) FROM early_warnning_setting");
                sql.Append(" WHERE is_del!=1");
                if (eqpTypeID != null)
                {
                    sql.Append(" and equipment_type_id=" + eqpTypeID);
                }
                if (!string.IsNullOrEmpty(paramID))
                {
                    sql.Append(" and param_id='" + paramID + "'");
                }

                var count = await c.QueryFirstAsync<int>(sql.ToString());
                return count;
            });
        }

        public async  Task<List<EarlyWarnningExType>> ListEarlyWarnningExType()
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * FROM early_warnning_ex_type";
                var data = await c.QueryAsync<EarlyWarnningExType>(sql);
                return data.ToList();
            });
        }

        public async Task<List<PidTable>> ListEqpPropByEqpTypeID(int eqpTypeID)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT distinct a.prop, a.Des, a.UT FROM pid_table a"
                    + " JOIN equipment b on a.eqp_id=b.id"
                    + " WHERE b.eqp_type = @EqpTypeID";
                var list = await c.QueryAsync<PidTable>(sql,
                new
                {
                    EqpTypeID = eqpTypeID
                });

                return list.ToList();
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

        // 根据设备类型和参数查pid
        public async Task<List<PidTable>> ListPidTable(List<int> eqpTypeIDs, List<string> props)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT a.*, b.eqp_type FROM pid_table a"
                    + " JOIN equipment b on a.eqp_id=b.id AND b.eqp_type IN @EqpTypeID"
                    + " WHERE a.prop IN @Prop";
                var list = await c.QueryAsync<PidTable>(sql,
                new
                {
                    EqpTypeID = eqpTypeIDs,
                    Prop = props
                });

                return list.ToList();
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

    }    
}
