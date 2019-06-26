using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using MSS.API.Dao;
using MSS.API.Model.Data;
using System.Threading.Tasks;
using System.Linq;
using Dapper;

namespace MSS.API.Dao.Implement
{
    public class WarnningSettingRepo : BaseRepo, IWarnningSettingRepo<EarlyWarnningSetting>
    {
        public WarnningSettingRepo(DapperOptions options) : base(options) { }

        public async Task<EarlyWarnningSetting> SaveWarnningSetting(EarlyWarnningSetting setting)
        {
            return await WithConnection(async c =>
            {
                string sql = "INSERT INTO early_warnning_setting (equipment_type_id, param_id, param_name, param_unit, param_limit_upper, param_limit_lower, created_time, created_by, is_del)"
                            +" Values (@EquipmentTypeID, @ParamID, @ParamName, @ParamUnit, @ParamLimitUpper, @ParamLimitLower, @CreatedTime, @CreatedBy, @IsDel);";
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

        public async Task<EarlyWarnningSetting> UpdateWarnningSetting(EarlyWarnningSetting warnSet)
        {
            return await WithConnection(async c =>
            {
                string sql = "UPDATE early_warnning_setting SET parent_id = @ParentID, name = @Name, node_type = @NodeType,"
                            + " updated_by = @UpdatedBy, updated_time = @UpdatedTime WHERE ID = @ID;";
                await c.ExecuteAsync(sql,
                new
                {
                    // ID = node.ID,
                    // ParentID = node.ParentID,
                    // Name = node.Name,
                    // NodeType = node.NodeType,
                    // UpdatedBy = node.UpdatedBy,
                    // UpdatedTime = node.UpdatedTime
                });
                return warnSet;
            });
        }
    }    
}
