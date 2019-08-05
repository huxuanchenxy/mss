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
        // 保存预警
        Task<EarlyWarnning> SaveWarnning(EarlyWarnning warn);

        // 更新报警
        Task<EarlyWarnning> UpdateWarnning(EarlyWarnning warn);

        // 大中修通知
        Task<Notification> SaveNotification(Notification noti);

        // 更新报警
        Task<Notification> UpdateNotification(Notification noti);

        Task<Notification> GetNotificationByID(int notificationID);

        // 获取当前未处理的预警 orgID为null时返回所有数据
        Task<List<EarlyWarnning>> ListEarlyWarnningByOrg(int? orgID);
        // 获取当前未处理的通知 orgID为null时返回所有数据
        Task<List<Notification>> ListNotificationByOrg(int? orgID);

        // 获取此组织下所有pid
        Task<List<PidTable>> ListPidTableByOrg(int? orgID, int? eqpType);

        // 查询预警历史
        Task<PageData<EarlyWarnning>> ListEarlyWarningHistory(WarningParam param);
        // 查询预警历史
        Task<PageData<Notification>> ListNotificationHistory(NotificationParam param);
    }
}
