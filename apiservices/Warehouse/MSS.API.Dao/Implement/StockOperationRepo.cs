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
using Newtonsoft.Json;

namespace MSS.API.Dao.Implement
{
    public class StockOperationRepo : BaseRepo, IStockOperationRepo<StockOperation>
    {
        public StockOperationRepo(DapperOptions options) : base(options) { }

        public async Task<StockOperation> Save(StockOperation stockOperation)
        {
            return await WithConnection(async c =>
            {
                List<StockOperationDetail> sods = JsonConvert.DeserializeObject<List<StockOperationDetail>>(stockOperation.DetailList);
                IDbTransaction trans = c.BeginTransaction();
                string sql = " insert into stock_operation " +
                    " values (0,@OperationID,@Type,@Reason,@Warehouse, " +
                    " @FromWarehouse,@Remark,@Picker,@Supplier,@Agreement, " +
                    " @BudgetDept,@BudgetItems,@CreatedBy,@CreatedTime); ";
                sql += "SELECT LAST_INSERT_ID()";
                try
                {
                    int newid = await c.QueryFirstOrDefaultAsync<int>(sql, stockOperation, trans);
                    stockOperation.ID = newid;
                    foreach (var item in sods)
                    {
                        item.Operation = newid;
                    }
                    sql = " insert into stock_operation_detail " +
                    " values (0,@SpareParts,@OrderNo,@CountNo,@UnitPrice, " +
                    " @Amount,@Currency,@Invoice,@Purchase,@Repair, " +
                    " @Operation,@ExchangeRate,@TotalAmount,@Remark) ";
                    int ret = await c.ExecuteAsync(sql, sods, trans);
                    trans.Commit();
                    return stockOperation;
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new Exception(ex.ToString());
                }
            });
        }

        public async Task<object> GetPageByParm(StockOperationQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,u1.user_name,u2.user_name as pname,u3.user_name as sname, ")
                .Append(" dt.name,w1.name as wname,w2.name as fromWName,o.name as bname ")
                .Append(" FROM stock_operation a ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.picker=u2.id ")
                .Append(" left join user u3 on a.supplier=u3.id ")
                .Append(" left join dictionary_tree dt on a.reason=dt.id ")
                .Append(" left join warehouse w1 on a.warehouse=w1.id ")
                .Append(" left join warehouse w2 on a.from_warehouse=w1.id ")
                .Append(" left join org_tree o on a.budget_dept=o.id where 1=1 ");
                StringBuilder whereSql = new StringBuilder();
                if (parm.SearchReason!=null)
                {
                    whereSql.Append(" and a.reason = " + parm.SearchReason);
                }
                if (parm.SearchWarehouse != null)
                {
                    whereSql.Append(" and a.warehouse = " + parm.SearchWarehouse);
                }
                if (parm.SearchSupplier != null)
                {
                    whereSql.Append(" and a.supplier = " + parm.SearchSupplier);
                }
                if (parm.SearchStart != null)
                {
                    whereSql.Append(" and a.create_time >= " + parm.SearchStart);
                }
                if (parm.SearchEnd != null)
                {
                    whereSql.Append(" and a.create_time <= " + parm.SearchEnd);
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                var tmp = await c.QueryAsync<StockOperation>(sql.ToString());
                if (tmp != null && tmp.Count() > 0)
                {
                    List<StockOperation> ets = tmp.ToList();
                    int total = await c.QueryFirstOrDefaultAsync<int>(
                        "select count(*) from stock_operation a where 1=1 " + whereSql.ToString());
                    return new { rows = ets, total = total };
                }
                else
                {
                    return new { rows = new List<StockOperation>(), total = 0 };
                }
            });
        }

        public async Task<StockOperation> GetByID(int id)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,u1.user_name,u2.user_name as pname,u3.user_name as sname, ")
                .Append(" dt.name,w1.name as wname,w2.name as fromWName,o.name as bname ")
                .Append(" FROM stock_operation a ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.picker=u2.id ")
                .Append(" left join user u3 on a.supplier=u3.id ")
                .Append(" left join dictionary_tree dt on a.reason=dt.id ")
                .Append(" left join warehouse w1 on a.warehouse=w1.id ")
                .Append(" left join warehouse w2 on a.from_warehouse=w1.id ")
                .Append(" left join org_tree o on a.budget_dept=o.id where a.id=@id ");
                var result = await c.QueryFirstOrDefaultAsync<StockOperation>(
                    sql.ToString(), new { id = id });
                return result;
            });
        }

        public async Task<List<StockOperationDetail>> ListByOperation(int operation)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT sod.*,sp.name,sp.model,sp.unit,dt.name as cname FROM stock_operation_detail sod" +
                " LEFT JOIN spare_parts sp on sp.id = sod.spare_parts " +
                " LEFT JOIN dictionary_tree dt on dt.id = sod.currency WHERE sod.operation = @id";
                var result = await c.QueryAsync<StockOperationDetail>(
                    sql, new { id = operation });
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<StockOperationDetail>();
            });
        }
    }
}
