using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface IEquipmentTypeRepo<T> where T:BaseEntity
    {
        Task<EquipmentType> Save(EquipmentType eqpType);
        Task<int> Update(EquipmentType eqpType);
        Task<int> Delete(string[] ids, int userID);
        Task<object> GetPageByParm(EqpTypeQueryParm parm);
        Task<EquipmentType> GetByID(int id);
        Task<List<EquipmentType>> GetAll();
    }
}
