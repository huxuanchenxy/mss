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
        Task<int> DelMMoudleItemValue(int list);

        Task<int> SaveMModule(MaintenanceModule maintenanceModule);

        Task<int> SaveMList(MaintenanceList maintenanceList);
        Task<int> UpdateMList(int status, int user,int id);
        Task<int> SaveMDetail(List<MaintenancePlanDetail> maintenancePlanDetail);
        Task<MaintenanceListView> ListPage(MaintenanceListParm parm);

        Task<List<MaintenanceModuleItemAll>> ListItems(int id);
        Task<List<MaintenanceModuleItemAll>> ListValues(int id);
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
                        " values (0,@List,@Module,@Eqp,@Item,@ItemValue); ";
                sql += "SELECT LAST_INSERT_ID() ";
                int ret = await c.ExecuteAsync(sql, maintenanceModuleItemValues);
                return ret;
            });
        }
        /// <summary>
        /// 根据list删除数据，和批量插入一起使用，由于还要更新状态，所以直接在sevice事务，而不是dal
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task<int> DelMMoudleItemValue(int list)
        {
            return await WithConnection(async c =>
            {
                string sql = " delete FROM maintenance_module_item_value where list=@list ";
                int ret = await c.ExecuteAsync(sql, new { list});
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
                        " values (0,@Title,@Team,@PlanDate,@Location,@LocationBy,@Status,@Remark," +
                        " @CreatedBy,@CreatedTime,@UpdatedBy,@UpdatedTime); ";
                sql += "SELECT LAST_INSERT_ID() ";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, maintenanceList);
                return newid;
            });
        }

        public async Task<int> UpdateMList(int status,int user,int id)
        {
            return await WithConnection(async c =>
            {
                string sql = " update maintenance_list " +
                        " set status=@status,updated_by=@user,updated_time=@time where id=@id ";
                int newid = await c.ExecuteAsync(sql, new {status,user,time=DateTime.Now,id });
                return newid;
            });
        }

        public async Task<MaintenanceListView> ListPage(MaintenanceListParm parm)
        {
            return await WithConnection(async c =>
            {
                MaintenanceListView ret = new MaintenanceListView();
                string sql = "SELECT ml.*,ot.name as team_name,d.name,u2.user_name,u1.user_name as cname " +
                " FROM maintenance_list ml " +
                " left join org_tree ot on ot.id=ml.team " +
                " left join dictionary_tree d on d.id=ml.status " +
                " left join user u1 on u1.id=ml.created_by " +
                " left join user u2 on u2.id=ml.updated_by " +
                " where 1=1 ";
                string sqlwhere="";
                if (!string.IsNullOrWhiteSpace(parm.Title))
                {
                    sqlwhere += " and ml.title like '%" + parm.Title+"%' ";
                }
                if (parm.Status !=null)
                {
                    sqlwhere += " and ml.status="+ parm.Status;
                }
                sql = sql + sqlwhere + " order by "+ parm.sort + " "+parm.order
                +" limit " + (parm.page - 1) * parm.rows + "," + parm.rows;
                var tmp = await c.QueryAsync<MaintenanceList>(sql);
                if (tmp.Count() > 0)
                {
                    sql = "select count(*) FROM maintenance_list ml " + sqlwhere;
                    ret.total = await c.QueryFirstOrDefaultAsync<int>(sql);
                    ret.rows = tmp.ToList();
                }
                else
                {
                    ret.rows = new List<MaintenanceList>();
                    ret.total = 0;
                }
                return ret;
            });
        }

        #endregion

        #region MaintenancePlanDetail
        public async Task<int> SaveMDetail(List<MaintenancePlanDetail> maintenancePlanDetail)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into maintenance_plan_detail " +
                        " values (0,@List,@Detail,@PlanCode,@Count,@PMType); ";
                return await c.ExecuteAsync(sql, maintenancePlanDetail);
            });
        }
        #endregion

        #region MaintenanceModuleItemAll,根据检修单获取所有父项、子项、值
        /// <summary>
        /// 新建时查询主表为item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<MaintenanceModuleItemAll>> ListItems(int id)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT DISTINCT mm.id,mm.name,mmi.item,mi.item_name,mi.item_type, " +
                " mpd.count from maintenance_module_item mmi " +
                " left join maintenance_module mm on mm.id=mmi.module " +
                " LEFT JOIN maintenance_item mi on mi.id=mmi.item " +
                " right JOIN maintenance_plan_detail mpd on mpd.plan_code=mm.plan_code " +
                " where mpd.list=@id ";
                var tmp = await c.QueryAsync<MaintenanceModuleItemAll>(sql,new { id});
                if (tmp.Count() > 0)
                {
                    return tmp.ToList();
                }
                else
                {
                    return new List<MaintenanceModuleItemAll>();
                }
            });
        }

        /// <summary>
        /// 更新时查询主表为value
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<MaintenanceModuleItemAll>> ListValues(int id)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT DISTINCT mm.id,mm.name,mmiv.item,mi.item_name,mi.item_type, " +
                " mpd.count,mmiv.eqp,mmiv.item_value from maintenance_module_item_value mmiv " +
                " left join maintenance_module mm on mm.id=mmiv.module " +
                " LEFT JOIN maintenance_item mi on mi.id=mmiv.item " +
                " right JOIN maintenance_plan_detail mpd on mpd.plan_code=mm.plan_code " +
                " where mmiv.list=@id ";
                var tmp = await c.QueryAsync<MaintenanceModuleItemAll>(sql, new { id });
                if (tmp.Count() > 0)
                {
                    return tmp.ToList();
                }
                else
                {
                    return new List<MaintenanceModuleItemAll>();
                }
            });
        }

        #endregion

    }
}



