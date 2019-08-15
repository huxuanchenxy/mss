using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MSS.API.Dao.Interface
{
    public interface IWarnningSettingRepo<T> where T:BaseEntity
    {
        // 取所有设备信息
        Task<List<Equipment>> ListAllEquipment();
        Task<List<EarlyWarnningSetting>> ListAllWarnningSetting();
            
        // 查询所有pid
        Task<List<PidTable>> ListAllPid();

        // 插入pid表
        Task<int> SavePidTable(List<PidTable> data);
        // 删除指定pid
        Task<int> DeletePidTable(List<PidTable> data);
    }
}
