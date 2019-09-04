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
        #region 库存操作
        public async Task<int> Save(StockOperationSaveParm parm)
        {
            return await WithConnection(async c =>
            {
                StockOperation stockOperation = parm.stockOperation;
                List<StockOperationDetail> sods = parm.stockOperationDetails;
                IDbTransaction trans = c.BeginTransaction();
                int ret;
                string sql = " insert into stock_operation " +
                    " values (0,@OperationID,@Type,@Reason,@Warehouse, " +
                    " @FromWarehouse,@Remark,@Picker,@Supplier,@Agreement, " +
                    " @BudgetDept,@BudgetItems,@CreatedBy,@CreatedTime); ";
                sql += "SELECT LAST_INSERT_ID()";
                try
                {
                    int newid = await c.QueryFirstOrDefaultAsync<int>(sql, stockOperation, trans);
                    stockOperation.ID = newid;
                    int i = 0;
                    foreach (var item in sods)
                    {
                        item.Operation = newid;
                        sql = " insert into stock_operation_detail " +
                            " values (0,@SpareParts,@OrderNo,@CountNo,@UnitPrice, " +
                            " @Amount,@Currency,@Invoice,@LifeDate,@WorkingOrder,@Purchase,@Repair, " +
                            " @Operation,@ExchangeRate,@TotalAmount,@Remark); ";
                        sql += "SELECT LAST_INSERT_ID()";
                        ret = await c.QueryFirstOrDefaultAsync<int>(sql, item, trans);
                        parm.stockDetails[i].StockOperationDetail = ret;
                        i++;
                    }
                    if (parm.isAddStockDetails)
                    {
                        sql = " insert into stock_detail " +
                            " values (0,@SpareParts,@StockOperationDetail,@StockNo,@TroubleNo,@InStockNo, " +
                            " @InspectionNo,@RepairNo,@Warehouse,@Status) ";
                    }
                    else
                    {
                        sql = " update stock_detail set stock_no=@StockNo,inspection_no=@InspectionNo,repair_no=@RepairNo," +
                            " trouble_no=@TroubleNo,in_stock_no=@InStockNo, " +
                            " warehouse=@Warehouse,status=@Status where id = @id ";
                    }
                    ret = await c.ExecuteAsync(sql, parm.stockDetails, trans);

                    foreach (var item in parm.stocks)
                    {
                        if (item.isAdd)
                        {
                            sql = " insert into stock " +
                                " values (0,@SpareParts,@StockNo,@TroubleNo,@InStockNo, " +
                                " @InspectionNo,@RepairNo,@Warehouse,@Amount) ";
                        }
                        else
                        {
                            sql = " update stock set stock_no=@StockNo,inspection_no=@InspectionNo,repair_no=@RepairNo," +
                                " trouble_no=@TroubleNo,in_stock_no=@InStockNo, " +
                                " warehouse=@Warehouse,amount=@Amount where id = @id ";
                        }
                        ret = await c.ExecuteAsync(sql, item, trans);
                    }

                    foreach (var item in parm.stockSums)
                    {
                        if (item.isAdd)
                        {
                            sql = " insert into stock_sum " +
                                " values (0,@SpareParts,@StockNo,@TroubleNo,@InStockNo, " +
                                " @InspectionNo,@RepairNo,@Amount) ";
                        }
                        else
                        {
                            sql = " update stock_sum set stock_no=@StockNo,inspection_no=@InspectionNo,repair_no=@RepairNo," +
                                " trouble_no=@TroubleNo,in_stock_no=@InStockNo,amount=@Amount " +
                                " where id = @id ";
                        }
                        ret = await c.ExecuteAsync(sql, item, trans);
                    }
                    trans.Commit();
                    return ret;
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
                .Append(" dt.name,w1.name as wname,w2.name as fromWName,o.name as bname,ot.name as pdname ")
                .Append(" FROM stock_operation a ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.picker=u2.id ")
                .Append(" left join org_user ou on a.picker=ou.user_id and ou.is_del=0 ")
                .Append(" left join org_tree ot on ou.org_node_id=ot.id ")
                .Append(" left join user u3 on a.supplier=u3.id ")
                .Append(" left join dictionary_tree dt on a.reason=dt.id ")
                .Append(" left join warehouse w1 on a.warehouse=w1.id ")
                .Append(" left join warehouse w2 on a.from_warehouse=w2.id ")
                .Append(" left join org_tree o on a.budget_dept=o.id where 1=1 ");
                StringBuilder whereSql = new StringBuilder();
                if (parm.SearchType != null)
                {
                    whereSql.Append(" and a.type = " + parm.SearchType);
                }
                if (parm.SearchReason!=null)
                {
                    whereSql.Append(" and a.reason = " + parm.SearchReason);
                }
                if (parm.SearchFromWarehouse != null)
                {
                    whereSql.Append(" and a.from_warehouse = " + parm.SearchFromWarehouse);
                }
                if (parm.SearchWarehouse != null)
                {
                    whereSql.Append(" and a.warehouse = " + parm.SearchWarehouse);
                }
                if (parm.SearchSupplier != null)
                {
                    whereSql.Append(" and a.supplier = " + parm.SearchSupplier);
                }
                if (parm.SearchPicker != null)
                {
                    whereSql.Append(" and a.picker = " + parm.SearchPicker);
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
                .Append(" dt.name,w1.name as wname,w2.name as fromWName,o.name as bname,ot.name as pdname ")
                .Append(" FROM stock_operation a ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.picker=u2.id ")
                .Append(" left join org_user ou on a.picker=ou.user_id and ou.is_del=0 ")
                .Append(" left join org_tree ot on ou.org_node_id=ot.id ")
                .Append(" left join user u3 on a.supplier=u3.id ")
                .Append(" left join dictionary_tree dt on a.reason=dt.id ")
                .Append(" left join warehouse w1 on a.warehouse=w1.id ")
                .Append(" left join warehouse w2 on a.from_warehouse=w2.id ")
                .Append(" left join org_tree o on a.budget_dept=o.id where a.id=@id ");
                var result = await c.QueryFirstOrDefaultAsync<StockOperation>(
                    sql.ToString(), new { id = id });
                return result;
            });
        }

        #endregion

        #region 库存分页查询
        public async Task<StockSumView> GetStockSumPageByParm(StockSumQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                StockSumView ret = new StockSumView();
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,sp.name ")
                .Append(" FROM stock_sum a ")
                .Append(" left join spare_parts sp on a.spare_parts=sp.id where 1=1 ");
                StringBuilder whereSql = new StringBuilder();
                if (parm.SearchSpareParts != null)
                {
                    whereSql.Append(" and a.spare_parts = " + parm.SearchSpareParts);
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                var tmp = await c.QueryAsync<StockSum>(sql.ToString());
                if (tmp != null && tmp.Count() > 0)
                {
                    ret.rows = tmp.ToList();
                    ret.total = await c.QueryFirstOrDefaultAsync<int>(
                        "select count(*) from stock_sum a where 1=1 " + whereSql.ToString());
                }
                else
                {
                    ret.rows = new List<StockSum>();
                    ret.total = 0;
                }
                return ret;
            });
        }

        #endregion

        #region 根据不同的条件查询
        public async Task<List<StockSum>> ListBySPs(List<int> spareParts)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * FROM stock_sum WHERE spare_parts in @spareParts";
                var result = await c.QueryAsync<StockSum>(
                    sql, new { spareParts });
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<StockSum>();
            });
        }

        public async Task<List<Stock>> ListBySPsAndWH(List<int> spareParts,int warehouse)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * FROM stock WHERE spare_parts in @spareParts and warehouse=@warehouse";
                var result = await c.QueryAsync<Stock>(
                    sql, new { spareParts, warehouse });
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<Stock>();
            });
        }
        public async Task<List<Stock>> ListStockBySPs(List<int> spareParts)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT s.*,ss.name FROM stock s" +
                " left join warehouse ss on s.warehouse=ss.id " +
                " WHERE spare_parts in @spareParts";
                var result = await c.QueryAsync<Stock>(
                    sql, new { spareParts });
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<Stock>();
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

        public async Task<List<StockDetail>> ListStockDetailBySPs(List<int> spareParts)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT sd.*,so.created_time,sod.count_no,sod.life_date,sod.unit_price," +
                " w.name,f.name as sname,dt.name as cname FROM stock_detail sd" +
                " left join stock_operation_detail sod on sd.stock_operation_detail=sod.id " +
                " left join dictionary_tree dt on sod.currency=dt.id " +
                " left join stock_operation so on sod.operation=so.id " +
                " left join warehouse w on so.warehouse=w.id " +
                " left join firm f on so.supplier=f.id " +
                " WHERE sd.spare_parts in @ids";
                var result = await c.QueryAsync<StockDetail>(
                    sql, new { ids= spareParts });
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<StockDetail>();
            });
        }

        #endregion

        #region 删除前提判断
        public async Task<bool> hasSpareParts(string[] spareParts)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT count(id) from stock WHERE spare_parts in @ids and stock_no=0";
                var result = await c.QueryFirstOrDefaultAsync<int>(
                    sql, new { ids = spareParts });
                return result>0;
            });
        }

        public async Task<bool> hasWarehouse(string[] warehouse)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT count(id) from stock WHERE warehouse in @ids and stock_no=0";
                var result = await c.QueryFirstOrDefaultAsync<int>(
                    sql, new { ids = warehouse });
                return result > 0;
            });
        }
        #endregion
    }
}
