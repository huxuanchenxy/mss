using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface IEquipmentRepairHistoryRepo<T> where T : BaseEntity
    {
        Task<EquipmentRepairHistory> Save(EquipmentRepairHistory equipmentRepairHistory);
        Task<int> Update(EquipmentRepairHistory equipmentRepairHistory);
        Task<int> Delete(string[] ids);
        Task<object> GetPageByParm(EquipmentRepairHistoryQueryParm parm);
        Task<EquipmentRepairHistory> GetByID(int id);
    }
}
