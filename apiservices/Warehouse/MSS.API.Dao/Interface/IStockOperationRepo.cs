using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        Task<List<Stock>> ListBySPsAndWH(List<int> spareParts, int warehouse);
        Task<List<Stock>> ListStockBySPs(List<int> spareParts);
        Task<List<object>> ListByWH(int warehouse);
        Task<List<StockOperationDetail>> ListByOperationIn(int operation);
        Task<List<StockOperationDetail>> ListByOperationOutIn(int operation);
        Task<List<StockOperationDetail>> ListByOperationOut(int operation);
        Task<List<StockOperationDetail>> ListSODByIDs(List<int> ids);

        Task<List<StockDetail>> ListStockDetailBySPsAndWH(List<int> spareParts, int warehouse);
        Task<StockDetail> GetStockDetailByID(int id);
        Task<List<StockDetail>> ListStockDetailByIDs(List<int> ids);
        Task<List<StockDetail>> GetStockDetailByEntitys(List<string> entitys, int warehouse = 0);
        Task<List<StockDetail>> ListStockDetail();

        Task<List<StockDetail>> ListStockDetailByOperation(int operation);
        Task<List<object>> ListByReason(int reason);

        Task<bool> hasSpareParts(string[] spareParts);
        Task<bool> hasWarehouse(string[] warehouse);
    }
}
