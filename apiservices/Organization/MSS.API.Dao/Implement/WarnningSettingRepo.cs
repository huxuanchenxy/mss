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

        public async Task<EarlyWarnningSetting> SaveWarnningSetting(EarlyWarnningSetting setting)
        {
            return await WithConnection(async c =>
            {
                string sql = "INSERT INTO early_warnning_setting (equipment_type_id, param_id, param_name, param_unit, param_limit_upper, param_limit_lower, is_actived, created_time, created_by, is_del)"
                            +" Values (@EquipmentTypeID, @ParamID, @ParamName, @ParamUnit, @ParamLimitUpper, @ParamLimitLower, @IsActived, @CreatedTime, @CreatedBy, @IsDel);";
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
                string sql = "INSERT INTO early_warnning_setting_ex (early_warnning_id, param_id, param_limit_type, param_limit_value, created_time, created_by, is_del)"
                            + " Values (@EarlyWarnningID, @ParamID, @ParamLimitType, @ParamLimitValue, @CreatedTime, @CreatedBy, @IsDel);";
                int affectedRows = await c.ExecuteAsync(sql, settingEx);
                return affectedRows;
            });
        }

        public async Task<EarlyWarnningSetting> UpdateWarnningSetting(EarlyWarnningSetting setting)
        {
            return await WithConnection(async c =>
            {
                string sql = "UPDATE early_warnning_setting SET param_limit_upper = @ParamLimitUpper, param_limit_lower = @ParamLimitLower, is_actived = @IsActived"
                            + " updated_by = @UpdatedBy, updated_time = @UpdatedTime WHERE equipment_type_id=@EquipmentTypeID and param_id=@ParamID;";
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

        public async Task<List<EarlyWarnningSetting>> ListWarnningSettingByPage(int page, int size, string sort, string order)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT * FROM early_warnning_setting");
                if(!string.IsNullOrEmpty(sort) && !string.IsNullOrEmpty(order))
                {
                    sql.Append(" order by " + sort + " " + order);
                }
                sql.Append(" limit " + (page - 1) * size + "," + size);

                var list = await c.QueryAsync<EarlyWarnningSetting>(sql.ToString());
                return list.ToList();
            });
        }

        public async Task<int> Count()
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT count(*) FROM early_warnning_setting");

                var count = await c.QueryFirstAsync<int>(sql.ToString());
                return count;
            });
        }

    }    
}
