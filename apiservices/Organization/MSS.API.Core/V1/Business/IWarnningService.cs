using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Common;
using MSS.API.Model.DTO;
namespace MSS.API.Core.V1.Business
{
    public interface IWarnningService
    {
        // 获取所有未处理预警 orgID 为null 取所有
        Task<ApiResult> ListWarnningByOrg(int? orgID);
        // 获取所有未处理通知 orgID 为null 取所有
        Task<ApiResult> ListNotificationByOrg(int? orgID);
        Task<ApiResult> ListAlarmByOrg(int? orgID);

        // 获取报警后再取设备信息
        Task<ApiResult> ListAlarmEqpByOrg(int? orgID, AlarmEqpParam param);

        // 查询报警历史记录
        Task<ApiResult> ListAlarmHistory(AlarmParam param);

        // 查询预警历史记录
        Task<ApiResult> ListEarlyWarningHistory(WarningParam param);
        // 查询通知记录
        Task<ApiResult> ListNotificationHistory(NotificationParam param);

        // 删除通知
        Task<ApiResult> DeleteNotification(int notificationId);
    }
}
