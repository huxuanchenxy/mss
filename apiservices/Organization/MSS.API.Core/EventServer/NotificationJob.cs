using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using MSS.Common.Consul;
using MSS.API.Common.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System.Collections.Generic;
using MSS.API.Core.Models.Ex;
using Microsoft.Extensions.Caching.Distributed;
using MSS.API.Core.Common;
using System.Threading;
using MSS.API.Common;
namespace MSS.API.Core.EventServer
{
    public class NotificationJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        private readonly GlobalDataManager _globalDataManager;
        private readonly IWarnningRepo<EarlyWarnning> _warnSetting;
        private readonly IDistributedCache _cache;
        private readonly EventQueues _queues;
        public NotificationJob(ILogger<WarningJob> logger, GlobalDataManager globalDataManager,
            IServiceDiscoveryProvider consulServiceProvider, IDistributedCache cache,
            IWarnningRepo<EarlyWarnning> warnSetting, EventQueues queues)
        {
            _logger = logger;
            _globalDataManager = globalDataManager;
            _consulServiceProvider = consulServiceProvider;
            _cache = cache;
            _warnSetting = warnSetting;
            _queues = queues;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            while(true)
            {
                try
                {
                    // 取大中修事件
                    List<EqpHistory> eqpHistory = new List<EqpHistory>();
                    var _services = await _consulServiceProvider.GetServiceAsync("ExpertDataAndDeviceService");
                    IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                    string types = (int)MyDictionary.EqpHistoryType.Install + "," 
                        + (int)MyDictionary.EqpHistoryType.MediumMaintenance + ","
                        + (int)MyDictionary.EqpHistoryType.MajorMaintenance;
                    ApiResult result = await httpHelper.
                        GetSingleItemRequest(_services + "/api/v1/EqpHistory/ListByType/" + types);
                    if (result.code == Code.Success)
                    {
                        eqpHistory = JsonConvert.DeserializeObject<List<EqpHistory>>(result.data.ToString());
                    }
                    // 循环设备判断寿命及大中修事件
                    List<Equipment> allEqp = _globalDataManager.AllEqp;
                    EquipmentConfig eqpConfig = _globalDataManager.EqpConfig;
                    foreach (Equipment eqp in allEqp)
                    {
                        // 寿命
                        _checkLifeCycle(eqp, eqpConfig);
                        _checkMediumMaintenance(eqp, eqpConfig, eqpHistory);
                        _checkMajorMaintenance(eqp, eqpConfig, eqpHistory);
                    }
                    Thread.Sleep(30000);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.StackTrace);
                }
            }
        }

        private void _checkMediumMaintenance(Equipment eqp, EquipmentConfig eqpConfig, List<EqpHistory> eqpHistory)
        {
            DateTime? start = null;
            EqpHistory his_medium = eqpHistory.Where(c => c.Type == 
                MyDictionary.EqpHistoryType.MediumMaintenance && c.EqpID == eqp.ID).FirstOrDefault();
            if (his_medium != null)
            {
                start = his_medium.CreatedTime;
            }
            else
            {
                EqpHistory his_install = eqpHistory.Where(c => c.Type ==
                MyDictionary.EqpHistoryType.Install && c.EqpID == eqp.ID).FirstOrDefault();
                if (his_install != null)
                {
                    start = his_install.CreatedTime;
                }
            }

            if (start != null && eqpConfig != null)
            {
                DateTime dateRepair = ((DateTime)start).AddDays(eqp.MediumRepair);
                if (dateRepair < DateTime.Now.AddDays(eqpConfig.beforeMaintainMiddle))
                {
                    string prefix = RedisKeyPrefix.Notification;
                    string notiID = _cache.GetString(prefix + eqp.ID + "_" + 2);
                    if (notiID == null)
                    {
                        int remain = (int)(dateRepair - DateTime.Now).TotalDays;
                        string msg = String.Format("设备中修时间{0}{1}天",
                            (remain > 0) ? "还有" : "超过", Math.Abs(remain));
                        
                        _addNotification(eqp, msg, 2, "中修");
                    }
                }
            }
        }

        private void _checkMajorMaintenance(Equipment eqp, EquipmentConfig eqpConfig, List<EqpHistory> eqpHistory)
        {
            DateTime? start = null;
            EqpHistory his_major = eqpHistory.Where(c => c.Type ==
                MyDictionary.EqpHistoryType.MajorMaintenance && c.EqpID == eqp.ID).FirstOrDefault();
            if (his_major != null)
            {
                start = his_major.CreatedTime;
            }
            else
            {
                EqpHistory his_install = eqpHistory.Where(c => c.Type ==
                MyDictionary.EqpHistoryType.Install && c.EqpID == eqp.ID).FirstOrDefault();
                if (his_install != null)
                {
                    start = his_install.CreatedTime;
                }
            }

            if (start != null && eqpConfig != null)
            {
                DateTime dateRepair = ((DateTime)start).AddDays(eqp.LargeRepair);
                if (dateRepair < DateTime.Now.AddDays(eqpConfig.beforeMaintainBig))
                {
                    string prefix = RedisKeyPrefix.Notification;
                    string notiID = _cache.GetString(prefix + eqp.ID + "_" + 1);
                    if (notiID == null)
                    {
                        int remain = (int)(dateRepair - DateTime.Now).TotalDays;
                        string msg = String.Format("设备大修时间{0}{1}天",
                            (remain > 0) ? "还有" : "超过", Math.Abs(remain));
                        _addNotification(eqp, msg, 1, "大修");
                    }
                }
            }
        }

        private void _checkLifeCycle(Equipment eqp, EquipmentConfig eqpConfig)
        {
            DateTime? start = null;
            if (eqp.OnlineAgain != null && eqp.OnlineAgain > eqp.OnlineDate)
            {
                start = eqp.OnlineAgain;
            }
            else
            {
                start = eqp.OnlineDate;
            }
            if (start != null && eqpConfig != null)
            {
                DateTime datelife = ((DateTime)start).AddYears(eqp.LifeCycle);
                if (datelife < DateTime.Now.AddDays(eqpConfig.beforeDead))
                {
                    string prefix = RedisKeyPrefix.Notification;
                    string notiID = _cache.GetString(prefix + eqp.ID + "_" + 0);
                    if (notiID == null)
                    {
                        int remain = (int)(datelife - DateTime.Now).TotalDays;
                        string msg = String.Format("寿命{0}{1}天",
                            (remain > 0) ? "还有" : "超过", Math.Abs(remain));
                        _addNotification(eqp, msg, 0, "寿命");
                    }
                }
            }
        }

        private async void _addNotification(Equipment eqp, string content, int notificationType,
            string notificationTypeName)
        {
            Notification noti = new Notification();
            noti.EqpID = eqp.ID;
            noti.Content = content;
            noti.NotificationType = notificationType;
            noti.NotificationTypeName = notificationTypeName;
            noti.Status = 0;
            noti.CreatedTime = DateTime.Now;
            Notification ret = await _warnSetting.SaveNotification(noti);
            // 插入redis
            string prefix = RedisKeyPrefix.Notification;
            // key为设备id_通知类型,value 为 此通知在通知表中的id
            _cache.SetString(prefix + eqp.ID + "_" + notificationType, ret.ID.ToString());

            _sendNotification(eqp.ID, "on");
        }

        private void _sendNotification(int eqpID, string content)
        {
            MssEventMsg msg = new MssEventMsg();
            msg.msg = content;
            msg.type = MssEventType.Notification;
            msg.eqp = new Equipment()
            {
                ID = eqpID
            };
            _queues.AlarmQueue.Enqueue(msg);
        }

    }
}