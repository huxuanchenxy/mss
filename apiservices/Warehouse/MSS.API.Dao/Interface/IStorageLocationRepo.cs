using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface IStorageLocationRepo<T> where T : BaseEntity
    {
        Task<StorageLocation> Save(StorageLocation storageLocation);
        Task<int> Update(StorageLocation storageLocation);
        Task<int> Delete(string[] ids, int userID);
        Task<object> GetPageByParm(StorageLocationQueryParm parm);
        Task<StorageLocation> GetByID(int id);
        Task<List<StorageLocation>> GetAll();
        Task<List<StorageLocation>> ListByWarehouse(int warehouse);
        Task<bool> HasByWarehouse(string[] warehouse);
    }
}
