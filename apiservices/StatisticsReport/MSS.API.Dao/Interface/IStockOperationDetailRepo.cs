using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MSS.API.Model.DTO;
using MSS.API.Dao.Common;
namespace MSS.API.Dao.Interface
{
    public interface IStockOperationDetailRepo<T> where T : BaseEntity
    {
        //Task<StockOperationDetailPageView> GetPageList(StockOperationDetailParm param);
        Task<List<StockOperationDetail>> GetByParm(StockOperationDetailParm parm);
        Task<StockOperationDetail> Save(StockOperationDetail obj);
        Task<StockOperationDetail> GetByID(long id);
        Task<int> Update(StockOperationDetail obj);
        Task<int> Delete(string[] ids, int userID);
    }
}
