using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface IFirmRepo<T> where T:BaseEntity
    {
        Task<Firm> Save(Firm eqpType);
        Task<int> Update(Firm eqpType);
        Task<int> Delete(string[] ids, int userID);
        Task<object> GetPageByParm(EqpTypeQueryParm parm);
        Task<Firm> GetByID(int id);
        Task<List<Firm>> GetAll();
    }
}
