using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface IEquipmentConfigRepo<T> where T:BaseEntity
    {
        Task<EquipmentConfig> Save(EquipmentConfig obj);
        Task<int> Update(EquipmentConfig obj);
        Task<int> Delete(string[] ids, int userID);
        //Task<object> GetPageByParm(EqpTypeQueryParm parm);
        Task<EquipmentConfig> GetByID(int id);
        Task<List<EquipmentConfig>> GetAll();
    }
}
