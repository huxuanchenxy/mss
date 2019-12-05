using Dapper;
using MSS.API.Common;
using MSS.Platform.Workflow.WebApi.Model;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Coded By admin 2019/9/27 11:18:53
namespace MSS.Platform.Workflow.WebApi.Data
{
    public interface IMaintenanceRepo<T>
    {
        Task<int> SaveMItem(MaintenanceItem maintenanceItem);

        Task<int> SaveMMoudleItem(List<MaintenanceModuleItem> maintenanceModuleItem);

        Task<int> SaveMMoudleItemValue(List<MaintenanceModuleItemValue> maintenanceModuleItemValues);

        Task<int> SaveMModule(MaintenanceModule maintenanceModule);
    }

    public class MaintenanceRepo : BaseRepo, IMaintenanceRepo<MaintenanceItem>
    {
        public MaintenanceRepo(DapperOptions options) : base(options)
        {
        }
        #region MaintenanceItem
        public async Task<int> SaveMItem(MaintenanceItem maintenanceItem)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into maintenance_item " +
                        " values (0,@ItemName,@ItemType); ";
                sql += "SELECT LAST_INSERT_ID() ";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, maintenanceItem);
                return newid;
            });
        }
        #endregion

        #region MaintenanceModuleItem
        public async Task<int> SaveMMoudleItem(List<MaintenanceModuleItem> maintenanceModuleItem)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into maintenance_module_item " +
                        " values (0,@Module,@Item); ";
                sql += "SELECT LAST_INSERT_ID() ";
                return await c.ExecuteAsync(sql, maintenanceModuleItem);
            });
        }
        #endregion

        #region MaintenanceModuleItemValue
        public async Task<int> SaveMMoudleItemValue(List<MaintenanceModuleItemValue> maintenanceModuleItemValues)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into maintenance_module_item_value " +
                        " values (0,@ModuleItem,@ItemValue); ";
                sql += "SELECT LAST_INSERT_ID() ";
                int ret = await c.ExecuteAsync(sql, maintenanceModuleItemValues);
                return ret;
            });
        }
        #endregion

        #region MaintenanceModule
        public async Task<int> SaveMModule(MaintenanceModule maintenanceModule)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into maintenance_module " +
                        " values (0,@Name,@Type,@PlanCode); ";
                sql += "SELECT LAST_INSERT_ID() ";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, maintenanceModule);
                return newid;
            });
        }
        #endregion

        #region MaintenanceList
        public async Task<int> SaveMList(MaintenanceList maintenanceList)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into maintenance_list " +
                        " values (0,@Team,@PMType,@PlanDate,@Status,@CreatedBy,@CreatedTime,@UpdateBy,@UpdateTime); ";
                sql += "SELECT LAST_INSERT_ID() ";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, maintenanceList);
                return newid;
            });
        }
        #endregion

    }
}



