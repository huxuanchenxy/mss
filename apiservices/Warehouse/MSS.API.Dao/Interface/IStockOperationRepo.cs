using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static MSS.API.Model.Data.Common;

namespace MSS.API.Dao.Interface
{
    public interface IStockOperationRepo<T>
    {
        Task<int> Save(StockOperationSaveParm parm);
        Task<object> GetPageByParm(StockOperationQueryParm parm);
        Task<StockOperation> GetByID(int id);
        Task<bool> HasEntity(List<string> entity);

        Task<StockSumView> GetStockSumPageByParm(StockSumQueryParm parm);
        Task<object> GetStockPageByParm(StockSumQueryParm parm);

        Task<List<StockSum>> ListBySPs(List<int> spareParts);
        Task<int> UpdateStockSumAlarm(List<int> spareParts, int isAlarm);
        Task<List<Stock>> ListBySPsAndWH(List<int> spareParts, int warehouse,List<int> storageLocation=null);
        Task<List<Stock>> ListStockBySPs(List<int> spareParts);
        Task<List<object>> ListByWH(int warehouse);
        Task<int> UpdateStockAlarm(List<int> spareParts, int isAlarm);
        Task<List<StockOperationDetail>> ListByOperationIn(int operation, bool hasStockNoZero = false);
        Task<List<StockOperationDetail>> ListByOperationOutIn(int operation);
        Task<List<StockOperationDetail>> ListByOperationOut(int operation,bool isEdit=false);
        Task<List<StockOperationDetail>> GetByOperationDetail(List<int> operationDetail);
        Task<List<StockOperationDetail>> ListSODByIDs(List<int> ids);

        Task<List<StockDetail>> ListStockDetailBySPsAndWH(List<int> spareParts, 
            int warehouse, bool hasStockNoZero = false,
            StockOptDetailType reason = StockOptDetailType.Distribution,int storageLocation=0);
        Task<StockDetail> GetStockDetailByID(int id);
        Task<List<StockDetail>> ListStockDetailByIDs(List<int> ids);
        Task<List<StockDetail>> GetStockDetailByEntitys(List<string> entitys, int warehouse = 0);
        Task<List<StockDetail>> GetStockDetailByEntityIDs(List<int> ids, int warehouse = 0);
        Task<List<StockDetail>> ListStockDetail(StockOptDetailType reason);

        Task<List<StockDetail>> ListStockDetailByOperation(int operation);
        Task<List<object>> ListByReason(int reason);

        Task<bool> hasSpareParts(string[] spareParts);
        Task<bool> hasWarehouse(string[] warehouse);
    }
}
