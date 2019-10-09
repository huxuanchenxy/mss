using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface IWarehouseAlarmRepo<T> where T:BaseEntity
    {
        Task<WarehouseAlarm> Save(WarehouseAlarm warehouseAlarm);
        Task<int> Update(WarehouseAlarm warehouseAlarm);
        Task<int> Delete(string[] ids);
        Task<object> GetPageByParm(WarehouseAlarmQueryParm parm);
        Task<List<WarehouseAlarm>> GetBySPsAndWH(List<int> spartParts, int warehouse);
        Task<WarehouseAlarm> GetByID(int id);
        Task<int> DeleteBySPs(string[] ids);
    }
}
