using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MSS.API.Model.DTO;
using MSS.API.Dao.Common;
namespace MSS.API.Dao.Interface
{
    public interface IWarnningRepo<T> where T:BaseEntity
    {
        // 大中修通知
        Task<Notification> SaveNotification(Notification noti);

        // 保存预警
        Task<EarlyWarnning> SaveWarnning(EarlyWarnning warn);

        // 更新报警
        Task<EarlyWarnning> UpdateWarnning(EarlyWarnning warn);
    }
}
