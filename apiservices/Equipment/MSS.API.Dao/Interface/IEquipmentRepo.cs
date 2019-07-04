using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface IEquipmentRepo<T> where T:BaseEntity
    {
        Task<Equipment> Save(Equipment eqp);
        Task<int> Update(Equipment eqp);
        Task<int> Delete(string[] ids, int userID);
        Task<object> GetPageByParm(EqpQueryParm parm);
        Task<Equipment> GetByID(int id);
        Task<List<Equipment>> GetAll();
    }
}
