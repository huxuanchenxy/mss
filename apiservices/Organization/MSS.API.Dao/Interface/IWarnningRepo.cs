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
        // 获取当前未处理的预警 orgID为null时返回所有数据
        Task<List<EarlyWarnning>> ListEarlyWarnningByOrg(int? orgID);
        // 获取当前未处理的通知 orgID为null时返回所有数据
        Task<List<Notification>> ListNotificationByOrg(int? orgID);

        // 查询预警历史
        Task<PageData<EarlyWarnning>> ListEarlyWarningHistory(WarningParam param);
        // 查询预警历史
        Task<PageData<Notification>> ListNotificationHistory(NotificationParam param);

    }
}
