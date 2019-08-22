using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface IStockOperationRepo<T>
    {
        Task<StockOperation> Save(StockOperation stockOperation);
        Task<object> GetPageByParm(StockOperationQueryParm parm);
        Task<StockOperation> GetByID(int id);
        Task<List<StockOperationDetail>> ListByOperation(int operation);
    }
}
