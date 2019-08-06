using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface IFirmRepo<T> where T:BaseEntity
    {
        Task<Firm> Save(Firm firm);
        Task<int> Update(Firm firm);
        Task<int> Delete(string[] ids, int userID);
        Task<object> GetPageByParm(FirmQueryParm parm);
        Task<Firm> GetByID(int id);
        Task<List<Firm>> GetAll();
    }
}
