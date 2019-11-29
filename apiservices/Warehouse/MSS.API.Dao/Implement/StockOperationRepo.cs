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
using static MSS.API.Model.Data.Common;
using System.Dynamic;

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
                int ret=0;
                string sql = " insert into stock_operation " +
                    " values (0,@OperationID,@Type,@Reason,@Warehouse, " +
                    " @FromStockOperation,@ToWarehouse,@Remark,@Picker,@Agreement, " +
                    " @BudgetDept,@BudgetDeptPath,@BudgetItems,@CreatedBy,@CreatedTime); ";
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
                            " values (0,@Entity,@SpareParts,@OrderNo,@CountNo,@UnitPrice, " +
                            " @Amount,@Currency,@StorageLocation,@FromStorageLocation,@Invoice," +
                            " @LifeDate,@Supplier,@WorkingOrder,@Purchase,@Repair,@SomeOrder, " +
                            " @Operation,@ExchangeRate,@TotalAmount,@Remark,@StockDetail,@FromStockOperationDetail," +
                            " @ReturnNo,@Status); ";
                        sql += "SELECT LAST_INSERT_ID()";
                        ret = await c.QueryFirstOrDefaultAsync<int>(sql, item, trans);
                        if (parm.stockDetails != null && parm.stockDetails.Count > 0)
                        {
                            parm.stockDetails[i].StockOperationDetail = ret;
                        }
                        i++;
                    }

                    if (parm.stockOperationDetailsUpdate!=null && parm.stockOperationDetailsUpdate.Count>0)
                    {
                        sql = " update stock_operation_detail set return_no=@ReturnNo,status=@Status where id = @id ";
                        ret = await c.ExecuteAsync(sql, parm.stockOperationDetailsUpdate, trans);
                    }

                    if (parm.stockDetails != null && parm.stockDetails.Count > 0)
                    {
                        sql = " insert into stock_detail " +
                            " values (0,@Entity,@SpareParts,@StockOperationDetail,@StockNo,@TroubleNo,@InStockNo, " +
                            " @InspectionNo,@RepairNo,@LentNo,@ScrapNo,@Warehouse,@StorageLocation,@IsBatch,@Status) ";
                        ret = await c.ExecuteAsync(sql, parm.stockDetails, trans);
                    }
                    if (parm.stockDetailsUpdate != null && parm.stockDetailsUpdate.Count > 0)
                    {
                        sql = " update stock_detail set stock_no=@StockNo,inspection_no=@InspectionNo,repair_no=@RepairNo," +
                            " trouble_no=@TroubleNo,in_stock_no=@InStockNo,lent_no=@LentNo,scrap_no=@ScrapNo, " +
                            " warehouse=@Warehouse,storage_location=@StorageLocation,status=@Status where id = @id ";
                        ret = await c.ExecuteAsync(sql, parm.stockDetailsUpdate, trans);
                    }
                    
                    ////移库专用
                    //if (parm.stockDetailsAdd != null && parm.stockDetailsAdd.Count > 0)
                    //{
                    //    sql = " insert into stock_detail " +
                    //        " values (0,@Entity,@SpareParts,@StockOperationDetail,@StockNo,@TroubleNo,@InStockNo, " +
                    //        " @InspectionNo,@RepairNo,@LentNo,@ScrapNo,@Warehouse,@Status) ";
                    //    ret = await c.ExecuteAsync(sql, parm.stockDetailsAdd, trans);
                    //}

                    foreach (var item in parm.stocks)
                    {
                        //入库时，可能第一次新增，后面更新这条在事务中本应新增的数据
                        if (item.ID==0)
                        {
                            sql = " insert into stock " +
                                " values (0,@SpareParts,@StockNo,@TroubleNo,@InStockNo, " +
                                " @InspectionNo,@RepairNo,@LentNo,@ScrapNo,@Warehouse,@StorageLocation,@Amount,@IsAlarm) ";
                        }
                        else
                        {
                            sql = " update stock set stock_no=@StockNo,inspection_no=@InspectionNo,repair_no=@RepairNo," +
                                " trouble_no=@TroubleNo,in_stock_no=@InStockNo,lent_no=@LentNo,scrap_no=@ScrapNo, " +
                                " warehouse=@Warehouse,storage_location=@StorageLocation,amount=@Amount,is_alarm=@IsAlarm " +
                                " where id = @id ";
                        }
                        ret = await c.ExecuteAsync(sql, item, trans);
                    }

                    foreach (var item in parm.stockSums)
                    {
                        if (item.IsAdd)
                        {
                            sql = " insert into stock_sum " +
                                " values (0,@SpareParts,@StockNo,@TroubleNo,@InStockNo, " +
                                " @InspectionNo,@RepairNo,@LentNo,@ScrapNo,@Amount,@IsAlarm) ";
                        }
                        else
                        {
                            sql = " update stock_sum set stock_no=@StockNo,inspection_no=@InspectionNo,repair_no=@RepairNo," +
                                " trouble_no=@TroubleNo,in_stock_no=@InStockNo,lent_no=@LentNo,scrap_no=@ScrapNo,amount=@Amount, " +
                                " is_alarm=@IsAlarm where id = @id ";
                        }
                        ret = await c.ExecuteAsync(sql, item, trans);
                    }
                    if (parm.wAlarmHistory.Count>0)
                    {
                        sql = " insert into warehouse_alarm_history " +
                        " values (0,@Warehouse,@SpareParts,@StockNo,@SafeStorage, " +
                        " @CreatedTime); ";
                        ret = await c.ExecuteAsync(sql, parm.wAlarmHistory, trans);
                    }
                    trans.Commit();
                    // 查询stock和stock_sum表，将其中0数据删除
                    // stcokNo、troubleNo、inStockNo、inspectionNo、repairNo，哪些为0？
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
                sql.Append("SELECT a.*,u1.user_name,u2.user_name as pname, ")
                .Append(" dt.name,w1.name as wname,w2.name as toWName,o.name as bname,ot.name as pdname ")
                .Append(" FROM stock_operation a ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.picker=u2.id ")
                .Append(" left join org_user ou on a.picker=ou.user_id and ou.is_del=0 ")
                .Append(" left join org_tree ot on ou.org_node_id=ot.id ")
                //.Append(" left join firm f on a.supplier=f.id ")
                .Append(" left join dictionary_tree dt on a.reason=dt.id ")
                .Append(" left join warehouse w1 on a.warehouse=w1.id ")
                .Append(" left join warehouse w2 on a.to_warehouse=w2.id ")
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
                if (parm.SearchToWarehouse != null)
                {
                    whereSql.Append(" and a.to_warehouse = " + parm.SearchToWarehouse);
                }
                if (parm.SearchWarehouse != null)
                {
                    whereSql.Append(" and a.warehouse = " + parm.SearchWarehouse);
                }
                if (parm.SearchAgreement != null)
                {
                    whereSql.Append(" and a.agreement like '%" + parm.SearchAgreement+"%' ");
                }
                if (parm.SearchPicker != null)
                {
                    whereSql.Append(" and a.picker = " + parm.SearchPicker);
                }
                if (parm.SearchStart != null)
                {
                    whereSql.Append(" and a.created_time >= '" + parm.SearchStart+"' ");
                }
                if (parm.SearchEnd != null)
                {
                    whereSql.Append(" and a.created_time <= '" + parm.SearchEnd + "' ");
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
                sql.Append("SELECT a.*,u1.user_name,u2.user_name as pname,so.operation_id as fsoName, ")
                .Append(" dt.name,w1.name as wname,w2.name as toWName,o.name as bname,ot.name as pdname ")
                .Append(" FROM stock_operation a ")
                .Append(" left join stock_operation so on a.from_stock_opetration=so.id ")
                .Append(" left join user u1 on a.created_by=u1.id ")
                .Append(" left join user u2 on a.picker=u2.id ")
                .Append(" left join org_user ou on a.picker=ou.user_id and ou.is_del=0 ")
                .Append(" left join org_tree ot on ou.org_node_id=ot.id ")
                .Append(" left join dictionary_tree dt on a.reason=dt.id ")
                .Append(" left join warehouse w1 on a.warehouse=w1.id ")
                .Append(" left join warehouse w2 on a.to_warehouse=w2.id ")
                .Append(" left join org_tree o on a.budget_dept=o.id where a.id=@id ");
                var result = await c.QueryFirstOrDefaultAsync<StockOperation>(
                    sql.ToString(), new { id = id });
                return result;
            });
        }

        public async Task<bool> HasEntity(List<string> entity)
        {
            return await WithConnection(async c =>
            {
                int result = await c.QueryFirstOrDefaultAsync<int>(
                    "select count(*) from stock_detail where entity in @entity", new { entity });
                return result>0;
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
                .Append(" FROM stock_sum a ");
                sql.Append(" left join spare_parts sp on a.spare_parts=sp.id where 1=1 ");
                StringBuilder whereSql = new StringBuilder();
                if (parm.SearchSpareParts != null)
                {
                    whereSql.Append(" and a.spare_parts = " + parm.SearchSpareParts);
                }
                if (parm.SearchIsAlarm)
                {
                    whereSql.Append(" and a.is_alarm = " + Convert.ToInt32(parm.SearchIsAlarm));
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

        public async Task<object> GetStockPageByParm(StockSumQueryParm parm)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT a.*,sp.name as spname,a.stock_no as editNo,sl.name as slname ")
                .Append(" FROM stock a ")
                .Append(" left join storage_location sl on sl.id=a.storage_location ")
                .Append(" left join spare_parts sp on a.spare_parts=sp.id where a.stock_no!=0 ");
                StringBuilder whereSql = new StringBuilder();
                if (parm.SearchSpareParts != null)
                {
                    whereSql.Append(" and a.spare_parts = " + parm.SearchSpareParts);
                }
                if (parm.SearchWarehouse != null)
                {
                    whereSql.Append(" and a.warehouse = " + parm.SearchWarehouse);
                }
                sql.Append(whereSql)
                .Append(" order by a." + parm.sort + " " + parm.order)
                .Append(" limit " + (parm.page - 1) * parm.rows + "," + parm.rows);
                var tmp = await c.QueryAsync<Stock>(sql.ToString());
                List<Stock> rows;
                int total;
                if (tmp != null && tmp.Count() > 0)
                {
                    rows = tmp.ToList();
                    total = await c.QueryFirstOrDefaultAsync<int>(
                        "select count(*) from stock a where 1=1 " + whereSql.ToString());
                    return new { rows, total };
                }
                else
                {
                    rows = new List<Stock>();
                    total = 0;
                }
                return new { rows, total };
            });
        }
        #endregion

        #region 各个库存相关表的操作
        #region StockSum
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

        public async Task<int> UpdateStockSumAlarm(List<int> spareParts, int isAlarm)
        {
            return await WithConnection(async c =>
            {
                string sql = "update stock_sum set is_alarm=@isAlarm where spare_parts in @spareParts";
                return await c.ExecuteAsync(sql, new { isAlarm, spareParts });
            });
        }
        #endregion

        #region Stock
        public async Task<List<Stock>> ListBySPsAndWH(List<int> spareParts,int warehouse,List<int> storageLocation=null)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT s.* FROM stock s " +
                //" left join warehouse ss on s.warehouse=ss.id " +
                "WHERE s.spare_parts in @spareParts and s.warehouse=@warehouse";
                object o = new { spareParts, warehouse };
                if (storageLocation!=null)
                {
                    sql += " and s.storage_location in @storageLocation";
                    o = new { spareParts, warehouse, storageLocation };
                }
                var result = await c.QueryAsync<Stock>(
                    sql, o);
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
                string sql = "SELECT s.*,ss.name,sl.name as slname FROM stock s" +
                " left join warehouse ss on s.warehouse=ss.id " +
                " left join storage_location sl on s.storage_location=sl.id " +
                " WHERE spare_parts in @spareParts and s.stock_no!=0";
                var result = await c.QueryAsync<Stock>(
                    sql, new { spareParts });
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<Stock>();
            });
        }

        public async Task<List<object>> ListByWH(int warehouse)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT distinct s.spare_parts,sp.name as spname FROM stock s " +
                " left join spare_parts sp on s.spare_parts=sp.id " +
                " WHERE s.warehouse=@warehouse ";
                var result = await c.QueryAsync<object>(
                    sql, new { warehouse });
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<object>();
            });
        }

        public async Task<int> UpdateStockAlarm(List<int> spareParts,int isAlarm)
        {
            return await WithConnection(async c =>
            {
                string sql = "update stock set is_alarm=@isAlarm where spare_parts in @spareParts";
                return await c.ExecuteAsync(sql, new { isAlarm, spareParts });
            });
        }

        #endregion

        #region StockOperationDetail
        /// <summary>
        /// 采购接收/其他接收的操作明细和报废
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public async Task<List<StockOperationDetail>> ListByOperationIn(int operation,bool hasStockNoZero = false)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT sod.*,sp.name,sp.model,sp.unit,dt.name as cname," +
                " f.name as sname,sl.name as slname,sd.in_stock_no " +
                " FROM stock_operation_detail sod" +
                " LEFT JOIN stock_detail sd on sd.id = sod.stock_detail " +
                " LEFT JOIN spare_parts sp on sp.id = sod.spare_parts " +
                " LEFT JOIN stock_operation so on so.id = sod.operation " +
                " LEFT JOIN firm f on f.id = sod.supplier " +
                " LEFT JOIN storage_location sl on sl.id = sod.storage_location " +
                " LEFT JOIN dictionary_tree dt on dt.id = sod.currency " +
                " WHERE sod.operation = @id ";
                if (!hasStockNoZero)
                {
                    sql += " and sd.stock_no!=0 ";
                }
                sql+=" order by sod.entity";
                var result = await c.QueryAsync<StockOperationDetail>(
                    sql, new { id = operation });
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<StockOperationDetail>();
            });
        }
        /// <summary>
        /// 出库后再入库
        /// 采购退货-->采购单
        /// 退料、故障件退库
        /// 借用、送修、送检归还
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public async Task<List<StockOperationDetail>> ListByOperationOutIn(int operation)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT sod.id, sod.entity, sod.spare_parts,sod.count_no,sod.order_no,sod.remark,sodInit.some_order,sodInit.working_order," +
                " sodInit.unit_price,sodInit.invoice,sodInit.life_date,f.name as sname," +
                " sp.name,sp.model,sp.unit,dt.name as cname,sl.name as slname " +
                " FROM stock_operation_detail sod" +
                " LEFT JOIN stock_operation_detail sodInit on sodInit.id = sod.stock_operation_detail " +
                " LEFT JOIN spare_parts sp on sp.id = sod.spare_parts " +
                " LEFT JOIN storage_location sl on sl.id = sod.storage_location " +
                " LEFT JOIN firm f on f.id = sodInit.supplier ";
                sql += " LEFT JOIN dictionary_tree dt on dt.id = sodInit.currency WHERE sod.operation = @id";
                var result = await c.QueryAsync<StockOperationDetail>(
                    sql, new { id = operation });
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<StockOperationDetail>();
            });
        }

        /// <summary>
        /// 除了采购接收和其他接收意外的操作
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public async Task<List<StockOperationDetail>> ListByOperationOut(int operation, bool isEdit = false)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT sod.id,sod.entity,sod.spare_parts,sod.count_no,sod.order_no,sod.remark,sod.some_order,sod.working_order," +
                " sodInit.unit_price,sodInit.invoice,sodInit.life_date,sodInit.exchange_rate,sod.stock_detail," +
                " sp.name,sp.model,sp.unit,dt.name as cname,f.name as sname,sod.return_no,sod.status,sd.is_batch, " +
                " sodInit.currency,sodInit.supplier,sod.storage_location as from_storage_location,sd.in_stock_no, " +
                " sod.storage_location,sl.name as slname,sod.stock_operation_detail,sdIn.entity as newentity, " +
                " sod.from_storage_location,fsl.name as fslname" +
                " FROM stock_operation_detail sod" +
                " LEFT JOIN stock_detail sd on sd.id = sod.stock_detail " +
                " LEFT JOIN stock_detail sdIn on sdIn.stock_operation_detail = sod.id " +
                " LEFT JOIN stock_operation_detail sodInit on sodInit.id = sd.stock_operation_detail " +
                //" LEFT JOIN stock_operation_detail sodReturn on sod.stock_operation_detail = sodReturn.id " +
                " LEFT JOIN spare_parts sp on sp.id = sod.spare_parts " +
                " LEFT JOIN stock_operation so on so.id = sodInit.operation " +
                " LEFT JOIN firm f on f.id = sodInit.supplier " +
                " LEFT JOIN storage_location sl on sl.id = sod.storage_location " +
                " LEFT JOIN storage_location fsl on fsl.id = sod.from_storage_location " +
                " LEFT JOIN dictionary_tree dt on dt.id = sodInit.currency " +
                " WHERE sod.operation = " + operation;
                if (isEdit)
                {
                    sql+=" and sod.count_no!=sod.return_no ";
                }
                sql+=" order by sod.entity";
                var result = await c.QueryAsync<StockOperationDetail>(sql);
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<StockOperationDetail>();
            });
        }

        /// <summary>
        /// 退料、故障件退库
        /// 借用、送修、送检归还
        /// 以上情况入库时，存入新库位专用
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public async Task<List<StockOperationDetail>> GetByOperationDetail(List<int> operationDetail)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT sod.id, sodInit.life_date,sodInit.currency,sodInit.supplier " +
                " FROM stock_operation_detail sod" +
                " LEFT JOIN stock_detail sd on sd.id = sod.stock_detail " +
                " LEFT JOIN stock_operation_detail sodInit on sodInit.id = sd.stock_operation_detail " +
                " WHERE sod.id in @id";
                var result = await c.QueryAsync<StockOperationDetail>(
                    sql, new { id = operationDetail });
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<StockOperationDetail>();
            });
        }

        public async Task<List<StockOperationDetail>> ListSODByIDs(List<int> ids)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * FROM stock_operation_detail WHERE id in @ids";
                var result = await c.QueryAsync<StockOperationDetail>(
                    sql, new { ids });
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<StockOperationDetail>();
            });
        }
        #endregion

        #region StockDetail
        public async Task<List<StockDetail>> ListStockDetailBySPsAndWH(List<int> spareParts,
            int warehouse,bool hasStockNoZero = false, 
            StockOptDetailType reason = StockOptDetailType.Distribution,int storageLocation=0)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT sd.*,so.created_time,sod.count_no,sod.life_date,sod.unit_price,sod.exchange_rate," +
                " sp.name as spname,w.name,f.name as sname,dt.name as cname,sod.remark,sl.name as slname, " +
                " sp.model,dt1.name as statusName FROM stock_detail sd" +
                " left join stock_operation_detail sod on sd.stock_operation_detail=sod.id " +
                " left join dictionary_tree dt on sod.currency=dt.id " +
                " left join dictionary_tree dt1 on sd.status=dt1.id " +
                " left join stock_operation so on sod.operation=so.id " +
                " left join warehouse w on sd.warehouse=w.id " +
                " left join firm f on sod.supplier=f.id " +
                " left join storage_location sl on sod.storage_location=sl.id " +
                " left join spare_parts sp on sd.spare_parts=sp.id " +
                " WHERE 1=1 ";
                dynamic obj=new ExpandoObject();
                if (spareParts!=null && spareParts.Count()>0)
                {
                    sql += " and sd.spare_parts in @ids ";
                    obj.ids = spareParts;
                }
                if (warehouse!=0)
                {
                    sql += " and sd.warehouse=@warehouse";
                    obj.warehouse= warehouse;
                }
                if (!hasStockNoZero)
                {
                    sql += " and sd.stock_no != @stockNo";
                    obj.stockNo = 0;
                }
                if (storageLocation!=0)
                {
                    sql += " and sd.storage_location= @storageLocation";
                    obj.storageLocation = storageLocation;
                }
                switch (reason)
                {
                    case StockOptDetailType.Inspection:
                    case StockOptDetailType.InspectionReturn:
                    case StockOptDetailType.lendReturn:
                    case StockOptDetailType.MaterialLend:
                    case StockOptDetailType.MaterialReturn:
                    case StockOptDetailType.RepairReceive:
                        sql += " and sd.is_batch=@isBacth and sd.status!=@status";
                        obj.status = (int)StockStatus.Trouble;
                        obj.isBacth = 0;
                        break;
                    case StockOptDetailType.TroubleRepair:
                    case StockOptDetailType.TroubleReturn:
                        sql += " and sd.is_batch=@isBacth and sd.status=@status";
                        obj.status = (int)StockStatus.Trouble;
                        obj.isBacth = 0;
                        break;
                    case StockOptDetailType.TroubleMoveTo:
                    case StockOptDetailType.TroubleMoveLocation:
                        sql += " and sd.status=@status";
                        obj.status = (int)StockStatus.Trouble;
                        break;
                }
                sql += " order by sd.entity";
                var result = await c.QueryAsync<StockDetail>(
                    sql, (object)obj);
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<StockDetail>();
            });
        }

        public async Task<StockDetail> GetStockDetailByID(int id)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT sd.*,so.created_time,sod.count_no,sod.life_date,sod.unit_price," +
                " sp.name as spname,w.name,f.name as sname,dt.name as cname,sod.remark,so.warehouse, " +
                " sp.model,sod.some_order,sod.working_order,sod.exchange_rate FROM stock_detail sd" +
                " left join stock_operation_detail sod on sd.stock_operation_detail=sod.id " +
                " left join dictionary_tree dt on sod.currency=dt.id " +
                " left join stock_operation so on sod.operation=so.id " +
                " left join warehouse w on so.warehouse=w.id " +
                " left join firm f on sod.supplier=f.id " +
                " left join spare_parts sp on sd.spare_parts=sp.id " +
                " WHERE sd.id = @id and sd.stock_no!=0";
                return await c.QueryFirstOrDefaultAsync<StockDetail>(
                    sql, new { id});
            });
        }

        public async Task<List<StockDetail>> GetStockDetailByEntitys(List<string> entitys,int warehouse=0)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT sd.*,so.created_time,sod.count_no,sod.life_date,sod.unit_price," +
                " sp.name as spname,w.name,f.name as sname,dt.name as cname,sod.remark,so.warehouse, " +
                " sp.model,sod.some_order,sod.working_order,sod.exchange_rate FROM stock_detail sd" +
                " left join stock_operation_detail sod on sd.stock_operation_detail=sod.id " +
                " left join dictionary_tree dt on sod.currency=dt.id " +
                " left join stock_operation so on sod.operation=so.id " +
                " left join warehouse w on so.warehouse=w.id " +
                " left join firm f on sod.supplier=f.id " +
                " left join spare_parts sp on sd.spare_parts=sp.id " +
                " WHERE sd.entity in @entitys ";
                object obj = new { entitys };
                if (warehouse != 0)
                {
                    sql += " and so.warehouse=@warehouse";
                    obj = new { entitys, warehouse };
                }
                var result = await c.QueryAsync<StockDetail>(
                    sql, obj);
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<StockDetail>();
            });
        }

        public async Task<List<StockDetail>> GetStockDetailByEntityIDs(List<int> ids, int warehouse = 0)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT sd.*,so.created_time,sod.count_no,sod.life_date,sod.unit_price," +
                " sp.name as spname,w.name,f.name as sname,dt.name as cname,sod.remark,so.warehouse, " +
                " sp.model,sod.some_order,sod.working_order,sod.exchange_rate FROM stock_detail sd" +
                " left join stock_operation_detail sod on sd.stock_operation_detail=sod.id " +
                " left join dictionary_tree dt on sod.currency=dt.id " +
                " left join stock_operation so on sod.operation=so.id " +
                " left join warehouse w on so.warehouse=w.id " +
                " left join firm f on sod.supplier=f.id " +
                " left join spare_parts sp on sd.spare_parts=sp.id " +
                " WHERE sd.id in @ids ";
                object obj = new { ids };
                if (warehouse != 0)
                {
                    sql += " and so.warehouse=@warehouse";
                    obj = new { ids, warehouse };
                }
                var result = await c.QueryAsync<StockDetail>(
                    sql, obj);
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<StockDetail>();
            });
        }

        /// <summary>
        /// 库存数量和操作数量直接有关，通过操作明细字段StockDetail关联查询
        /// 采购/其他接收、采购退货
        /// 存货/故障件报废
        /// 领料、借用、送修、送检
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<List<StockDetail>> ListStockDetailByIDs(List<int> ids)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * FROM stock_detail " +
                " WHERE id in @ids";
                var result = await c.QueryAsync<StockDetail>(
                    sql, new { ids});
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<StockDetail>();
            });
        }

        public async Task<List<StockDetail>> ListStockDetail(StockOptDetailType reason)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT * FROM stock_detail where stock_no!=0 ";
                switch(reason)
                {
                    case StockOptDetailType.InStockScrap:
                        sql += " and in_stock_no!=0";
                        break;
                    case StockOptDetailType.TroubleScrap:
                        sql += " and trouble_no!=0";
                        break;
                }
                sql += " order by entity";
                var result = await c.QueryAsync<StockDetail>(
                    sql);
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<StockDetail>();
            });
        }


        /// <summary>
        /// 目前仅用于采购退货
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        public async Task<List<StockDetail>> ListStockDetailByOperation(int operation)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT sd.*,so.created_time,sod.count_no,sod.life_date,sod.unit_price,sod.exchange_rate," +
                " sl.name as slname,dt.name as cname,sp.name as spname,f.name as sname,sod.id as fsod,w.name FROM stock_detail sd" +
                " left join stock_operation_detail sod on sd.stock_operation_detail=sod.id " +
                " left join firm f on sod.supplier=f.id " +
                " left join spare_parts sp on sod.spare_parts=sp.id " +
                " left join dictionary_tree dt on sod.currency=dt.id " +
                " left join stock_operation so on sod.operation=so.id " +
                " left join warehouse w on w.id=sd.warehouse " +
                " left join storage_location sl on sl.id=sd.storage_location " +
                " WHERE sod.operation=@id";
                var result = await c.QueryAsync<StockDetail>(
                    sql, new { id = operation });
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<StockDetail>();
            });
        }
        #endregion

        #region stock_operation
        /// <summary>
        /// 根据事务原因获取id-流水号的下拉列表
        /// 采购退货
        /// 领料、借用、送修、送检
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        public async Task<List<object>> ListByReason(int reason)
        {
            return await WithConnection(async c =>
            {
                //sod.count_no==1表示排除批量
                StringBuilder sql = new StringBuilder();
                sql.Append("SELECT DISTINCT so.id,so.operation_id ")
                .Append(" FROM stock_operation so ")
                .Append(" left join stock_operation_detail sod on sod.operation=so.id")
                .Append(" where so.reason=@reason and sod.count_no=1 and sod.count_no!=sod.return_no");
                StockOptDetailType r = (StockOptDetailType)reason;
                //// 全部退回/归还，除去此记录
                //if (r == StockOptDetailType.Distribution || r==StockOptDetailType.Inspection ||
                //r==StockOptDetailType.MaterialLend || r==StockOptDetailType.TroubleRepair)
                //{
                //    sql.Append(" and count_no > return_no");
                //}
                var result = await c.QueryAsync<object>(
                    sql.ToString(), new { reason });
                if (result != null && result.Count() > 0)
                {
                    return result.ToList();
                }
                return new List<object>();
            });
        }
        #endregion

        #endregion

        #region 删除前提判断
        public async Task<bool> hasSpareParts(string[] spareParts)
        {
            return await WithConnection(async c =>
            {
                string sql = "SELECT count(id) from stock WHERE spare_parts in @ids and stock_no!=0";
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
