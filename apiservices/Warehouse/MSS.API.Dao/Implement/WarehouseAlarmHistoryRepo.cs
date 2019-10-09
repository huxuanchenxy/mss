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
    public class WarehouseAlarmHistoryRepo : BaseRepo, IWarehouseAlarmHistoryRepo<WarehouseAlarmHistory>
    {
        public WarehouseAlarmHistoryRepo(DapperOptions options) : base(options) { }

        public async Task<WarehouseAlarmHistory> Save(WarehouseAlarmHistory warehouseAlarmHistory)
        {
            return await WithConnection(async c =>
            {
                string sql = " insert into warehouse_alarm_history " +
                    " values (0,@Warehouse,@SpareParts,@StockNo,@SafeStorage, " +
                    " @CreatedTime); ";
                sql += "SELECT LAST_INSERT_ID()";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, warehouseAlarmHistory);
                warehouseAlarmHistory.ID = newid;
                return warehouseAlarmHistory;
            });
        }


        public async Task<object> GetPageByParm(WarehouseAlarmHistoryQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,w.name,sp.name as spName ")
                .Append(" FROM warehouse_alarm_history a ")
                .Append(" left join warehouse w on a.warehouse=w.id ")
                .Append(" left join spare_parts sp on a.spare_parts=sp.id where 1=1 ");
                StringBuilder whereSql = new StringBuilder();
                if (parm.SearchSpareParts != null)
                {
                    whereSql.Append(" and a.spare_parts=" + parm.SearchSpareParts);
                }
                if (parm.SearchType!=null)
                {
                    whereSql.Append(" and a.warehouse=" + parm.SearchType);
                }
                if (parm.SearchStart != null)
                {
                    whereSql.Append(" and a.created_time >= '" + parm.SearchStart + "' ");
                }
                if (parm.SearchEnd != null)
                {
                    whereSql.Append(" and a.created_time <= '" + parm.SearchEnd + "' ");
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                List< WarehouseAlarmHistory > ets= (await c.QueryAsync<WarehouseAlarmHistory>(sql.ToString())).ToList();
                int total = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from warehouse_alarm_history a where 1=1 " + whereSql.ToString());
                return new {rows=ets,total=total };
            });
        }

    }
}
