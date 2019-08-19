using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface ISparePartsRepo<T> where T:BaseEntity
    {
        Task<SpareParts> Save(SpareParts spareParts);
        Task<int> Update(SpareParts spareParts);
        Task<int> Delete(string[] ids, int userID);
        Task<object> GetPageByParm(SparePartsQueryParm parm);
        Task<SpareParts> GetByID(int id);
        Task<List<SpareParts>> GetAll();
    }
}
