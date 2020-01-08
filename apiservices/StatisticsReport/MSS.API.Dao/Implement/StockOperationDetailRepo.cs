using Dapper;
using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


// Coded By admin 2019/11/29 17:09:18
namespace MSS.API.Dao.Implement
{
    public class StockOperationDetailRepo : BaseRepo, IStockOperationDetailRepo<StockOperationDetail>
    {
        public StockOperationDetailRepo(DapperOptions options) : base(options) { }

        public async Task<List<StockOperationDetail>> GetByParm(StockOperationDetailParm parm)
        {
            return await WithConnection(async c =>
            {
                StringBuilder sql = new StringBuilder();
                sql = sql.Append($@"  SELECT a.count_no CountNo ,a.unit_price UniPrice,a.amount Amount
                                        ,b.created_time CreatedTime,c.name Name,c.eqp_type EqpType,c.id as SparePartsId
                                        FROM stock_operation_detail a
                                        LEFT JOIN stock_operation b ON a.operation = b.id
                                        LEFT JOIN spare_parts c ON c.id = a.spare_parts WHERE 1= 1 ");

                StringBuilder whereSql = new StringBuilder();
                if (parm.StockOperationType != 0)
                {
                    whereSql.Append(" AND b.type = '" + parm.StockOperationType + "'");
                }
                if (parm.SparePartsType != 0)
                {
                    whereSql.Append(" AND c.type = '" + parm.SparePartsType + "'");
                }
                if (!string.IsNullOrEmpty(parm.startTime) && !string.IsNullOrEmpty(parm.endTime))
                {
                    whereSql.Append(" AND ( b.created_time >= '" + parm.startTime + "' AND b.created_time <= '" + parm.endTime + "' )");
                }


                sql.Append(whereSql + " ORDER BY b.created_time ASC ");

                List<StockOperationDetail> result = new List<StockOperationDetail>();
                var data = await c.QueryAsync<StockOperationDetail>(sql.ToString());
                result = data.ToList();
                return result;
            });
        }
        public async Task<StockOperationDetail> Save(StockOperationDetail obj)
        {
            return await WithConnection(async c =>
            {
                string sql = $@" INSERT INTO `stock_operation_detail`(
                    
                    entity,
                    spare_parts,
                    order_no,
                    count_no,
                    unit_price,
                    amount,
                    currency,
                    storage_location,
                    from_storage_location,
                    invoice,
                    life_date,
                    supplier,
                    working_order,
                    purchase,
                    repair,
                    some_order,
                    operation,
                    exchange_rate,
                    total_amount,
                    remark,
                    stock_detail,
                    stock_operation_detail,
                    return_no,
                    status
                ) VALUES 
                (
                    @Entity,
                    @SpareParts,
                    @OrderNo,
                    @CountNo,
                    @UnitPrice,
                    @Amount,
                    @Currency,
                    @StorageLocation,
                    @FromStorageLocation,
                    @Invoice,
                    @LifeDate,
                    @Supplier,
                    @WorkingOrder,
                    @Purchase,
                    @Repair,
                    @SomeOrder,
                    @Operation,
                    @ExchangeRate,
                    @TotalAmount,
                    @Remark,
                    @StockDetail,
                    @StockOperationDetail,
                    @ReturnNo,
                    @Status
                    );
                    ";
                sql += "SELECT LAST_INSERT_ID() ";
                int newid = await c.QueryFirstOrDefaultAsync<int>(sql, obj);
                obj.Id = newid;
                return obj;
            });
        }

        public async Task<StockOperationDetail> GetByID(long id)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryFirstOrDefaultAsync<StockOperationDetail>(
                    "SELECT * FROM stock_operation_detail WHERE id = @id", new { id = id });
                return result;
            });
        }

        public async Task<int> Update(StockOperationDetail obj)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync($@" UPDATE stock_operation_detail set 
                    
                    entity=@Entity,
                    spare_parts=@SpareParts,
                    order_no=@OrderNo,
                    count_no=@CountNo,
                    unit_price=@UnitPrice,
                    amount=@Amount,
                    currency=@Currency,
                    storage_location=@StorageLocation,
                    from_storage_location=@FromStorageLocation,
                    invoice=@Invoice,
                    life_date=@LifeDate,
                    supplier=@Supplier,
                    working_order=@WorkingOrder,
                    purchase=@Purchase,
                    repair=@Repair,
                    some_order=@SomeOrder,
                    operation=@Operation,
                    exchange_rate=@ExchangeRate,
                    total_amount=@TotalAmount,
                    remark=@Remark,
                    stock_detail=@StockDetail,
                    stock_operation_detail=@StockOperationDetail,
                    return_no=@ReturnNo,
                    status=@Status
                 where id=@Id", obj);
                return result;
            });
        }

        public async Task<int> Delete(string[] ids, int userID)
        {
            return await WithConnection(async c =>
            {
                var result = await c.ExecuteAsync(" Update stock_operation_detail set is_del=1" +
                ",updated_time=@updated_time,updated_by=@updated_by" +
                " WHERE id in @ids ", new { ids = ids, updated_time = DateTime.Now, updated_by = userID });
                return result;
            });
        }

        public async Task<List<SpareParts>> GetSpareParts(string ids)
        {
            return await WithConnection(async c =>
            {
                var result = await c.QueryAsync<SpareParts>(
                    " SELECT DISTINCT id,name,type FROM spare_parts WHERE  id in ("+ids+") ");
                return result.ToList();
            });
        }
    }
}



