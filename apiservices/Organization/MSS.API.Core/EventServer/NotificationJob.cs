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
        public Task Execute(IJobExecutionContext context)
        {
            while(true)
            {
                try
                {
                    // 取大中修事件

                    // 循环设备判断寿命及大中修事件
                    List<Equipment> allEqp = _globalDataManager.AllEqp;
                    EquipmentConfig eqpConfig = _globalDataManager.EqpConfig;
                    foreach (Equipment eqp in allEqp)
                    {
                        // 寿命
                        _checkLifeCycle(eqp, eqpConfig);
                    }
                    Thread.Sleep(30000);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.StackTrace);
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
                if (((DateTime)start).AddYears(eqp.LifeCycle) < DateTime.Now.AddDays(eqpConfig.beforeDead))
                {
                    string prefix = RedisKeyPrefix.Notification;
                    string notiID = _cache.GetString(prefix + eqp.ID + "_" + 0);
                    if (notiID == null)
                    {
                        _addNotification(eqp, "寿命还有" + eqpConfig.beforeDead + "天到期", 0, "寿命");
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