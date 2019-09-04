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

        Task<StockSumView> GetStockSumPageByParm(StockSumQueryParm parm);

        Task<List<StockSum>> ListBySPs(List<int> spareParts);
        Task<List<Stock>> ListBySPsAndWH(List<int> spareParts, int warehouse);
        Task<List<Stock>> ListStockBySPs(List<int> spareParts);
        Task<List<StockOperationDetail>> ListByOperation(int operation);
        Task<List<StockDetail>> ListStockDetailBySPs(List<int> spareParts);

        Task<bool> hasSpareParts(string[] spareParts);
        Task<bool> hasWarehouse(string[] warehouse);
    }
}
