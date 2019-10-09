using System;
using System.Collections.Generic;
using System.Text;
using MSS.API.Dao.Interface;
using MSS.API.Dao;
using MSS.API.Model.Data;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using MSS.API.Common;
using System.Data;

namespace MSS.API.Dao.Implement
{
    public class WarehouseAlarmRepo : BaseRepo, IWarehouseAlarmRepo<WarehouseAlarm>
    {
        public WarehouseAlarmRepo(DapperOptions options) : base(options) { }

        public async Task<WarehouseAlarm> Save(WarehouseAlarm warehouseAlarm)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into warehouse_alarm " +
                    " values (0,@Warehouse,@SpareParts,@SafeStorage, " +
                    " @CreatedTime,@CreatedBy,@UpdatedTime,@UpdatedBy,@IsDel); ";
                sql += "SELECT LAST_INSERT_ID()";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, warehouseAlarm);
                warehouseAlarm.ID = newid;
                return warehouseAlarm;
            });
        }

        public async Task<int> Update(WarehouseAlarm warehouseAlarm)
        {
            return await WithConnection(async c =>
            {
                int ret = await c.ExecuteAsync(" update warehouse_alarm " +
                    " set warehouse=@Warehouse,spare_parts=@SpareParts,safe_storage=@SafeStorage, " +
                    " updated_time=@UpdatedTime,updated_by=@UpdatedBy where id=@id", warehouseAlarm);
                return ret;
            });
        }

        public async Task<int> Delete(string[] ids)
        {
            return await WithConnection(async c =>
            {
                int ret = await c.ExecuteAsync(" Delete from warehouse_alarm " + 
                " WHERE id in @ids ", new { ids = ids });
                return ret;
            });
        }

        public async Task<int> DeleteBySPs(string[] ids)
        {
            return await WithConnection(async c =>
            {
                int ret = await c.ExecuteAsync(" Delete from warehouse_alarm " +
                " WHERE spare_parts in @ids ", new { ids = ids });
                return ret;
            });
        }


        public async Task<object> GetPageByParm(WarehouseAlarmQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,w.name,u1.user_name as created_name,u2.user_name as updated_name ")
                .Append(" FROM warehouse_alarm a ")
                .Append(" left join warehouse w on a.warehouse=w.id ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.updated_by=u2.id ");
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" WHERE a.is_del=" + (int)IsDeleted.no);
                whereSql.Append(" and a.spare_parts=" + parm.SearchSpareParts);
                if (parm.SearchType!=null)
                {
                    whereSql.Append(" and a.warehouse=" + parm.SearchType);
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                List< WarehouseAlarm > ets= (await c.QueryAsync<WarehouseAlarm>(sql.ToString())).ToList();
                int total = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from warehouse_alarm a " + whereSql.ToString());
                return new {rows=ets,total=total };
            });
        }

        public async Task<List<WarehouseAlarm>> GetBySPsAndWH(List<int> spartParts,int warehouse)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT * FROM warehouse_alarm ")
                .Append(" WHERE spare_parts in @spartParts and warehouse=@warehouse ");
                var result = await c.QueryAsync<WarehouseAlarm>(
                    sql.ToString(), new { spartParts, warehouse });
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<WarehouseAlarm>();
            });
        }

        public async Task<WarehouseAlarm> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<WarehouseAlarm>(
                    "select * from warehouse_alarm where id=@id", new { id = id });
                return result;
            });
        }

    }
}
