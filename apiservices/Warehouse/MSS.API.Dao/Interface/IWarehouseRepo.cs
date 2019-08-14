using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface IWarehouseRepo<T> where T:BaseEntity
    {
        Task<Warehouse> Save(Warehouse warehouse);
        Task<int> Update(Warehouse warehouse);
        Task<int> Delete(string[] ids, int userID);
        Task<object> GetPageByParm(WarehouseQueryParm parm);
        Task<Warehouse> GetByID(int id);
        Task<List<Warehouse>> GetAll();
    }
}
