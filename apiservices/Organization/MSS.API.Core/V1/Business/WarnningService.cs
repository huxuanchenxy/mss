using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using MSS.API.Model.DTO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MSS.API.Common;
using MSS.API.Core.Common;
using Microsoft.Extensions.Caching.Distributed;
using MSS.API.Core.EventServer;
using MSS.Common.Consul;
using MSS.API.Common.Utility;
using MSS.API.Core.Models.Ex;
namespace MSS.API.Core.V1.Business
{
    public class WarnningService : IWarnningService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IWarnningRepo<EarlyWarnning> _warnRepo;
        private readonly IDistributedCache _cache;
        private readonly EventQueues _queues;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;

        public WarnningService(IWarnningRepo<EarlyWarnning> warnRepo, IDistributedCache cache,
            EventQueues queues, IServiceDiscoveryProvider consulServiceProvider)
        {
            //_logger = logger;
            _warnRepo = warnRepo;
            _cache = cache;
            _queues = queues;
            _consulServiceProvider = consulServiceProvider;
        }

        public async Task<ApiResult> ListWarnningByOrg(int? orgID)
        {
            ApiResult ret = new ApiResult();
            try
            {
                var data = await _warnRepo.ListEarlyWarnningByOrg(orgID);
                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> ListNotificationByOrg(int? orgID)
        {
            ApiResult ret = new ApiResult();
            try
            {
                var data = await _warnRepo.ListNotificationByOrg(orgID);
                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> ListAlarmByOrg(int? orgID)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<PidTable> data = await _warnRepo.ListPidTableByOrg(orgID, null);

                // 获取alarm
                var services = await _consulServiceProvider.GetServiceAsync("RDBEssService");
                IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
                ApiResult result = await httpHelper.
                    GetSingleItemRequest(services + "/api/v1/Ealarm/0");
                if (result.code == Code.Success)
                {
                    List<Alarm> alarms = JsonConvert.DeserializeObject<List<Alarm>>(result.data.ToString());
                    foreach (Alarm alarm in alarms)
                    {
                        PidTable pid = data.Where(c => c.pid == alarm.pid).FirstOrDefault();
                        if (pid != null)
                        {
                            alarm.EqpCode = pid.EqpCode;
                            alarm.EqpName = pid.EqpName;
                            alarm.EqpTypeName = pid.EqpTypeName;
                        }
                    }
                    ret.code = Code.Success;
                    ret.data = alarms;
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> ListAlarmHistory(AlarmParam query)
        {
            switch (query.sort)
            {
                case "created_time":
                    query.sort = "eTime";
                    break;
            }
            ApiResult ret = new ApiResult();
            ret.code = Code.Failure;
            try
            {
                List<PidTable> data = await _warnRepo.ListPidTableByOrg(query.orgID, query.eqpTypeID);

                var param = new {
                    pidList = new List<object>()
                };
                foreach (PidTable pid in data)
                {
                    var p = new {
                        pid = pid.pid,
                        originTimeStart = query.startTime,
                        originTimeEnd = query.endTime,
                        page = query.page,
                        rows = query.rows,
                        sort = query.sort,
                        order = query.order
                    };
                    param.pidList.Add(p);
                }

                // 获取alarm
                var services = await _consulServiceProvider.GetServiceAsync("RDBEssService");
                ApiResult result = HttpClientHelper.PostResponse<ApiResult>(services +
                    "/api/v1/Elog/page", param);
                if (result.code == Code.Success)
                {
                    List<PageData<Alarm>> pageData = JsonConvert.DeserializeObject<List<PageData<Alarm>>>(result.data.ToString());
                    List<Alarm> allAlarm = new List<Alarm>();
                    int total = 0;
                    foreach (PageData<Alarm> page in pageData)
                    {
                        total += page.total;
                        allAlarm.AddRange(page.rows);
                    }
                    PageData<Alarm> retPageData = new PageData<Alarm>();
                    retPageData.total = total;
                    if (query.order.Equals("asc"))
                    {
                        retPageData.rows = allAlarm.OrderBy(c => c.ETime).Take(query.rows).ToList();
                    }
                    else
                    {
                        retPageData.rows = allAlarm.OrderByDescending(c => c.ETime).Take(query.rows).ToList();
                    }
                    foreach (Alarm alarm in retPageData.rows)
                    {
                        PidTable pid = data.Where(c => c.pid == alarm.pid).FirstOrDefault();
                        if (pid != null)
                        {
                            alarm.EqpCode = pid.EqpCode;
                            alarm.EqpName = pid.EqpName;
                            alarm.EqpTypeName = pid.EqpTypeName;
                        }
                    }

                    ret.code = Code.Success;
                    ret.data = retPageData;
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> ListEarlyWarningHistory(WarningParam param)
        {
            ApiResult ret = new ApiResult();
            try
            {
                var data = await _warnRepo.ListEarlyWarningHistory(param);
                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> ListNotificationHistory(NotificationParam param)
        {
            ApiResult ret = new ApiResult();
            try
            {
                var data = await _warnRepo.ListNotificationHistory(param);
                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> DeleteNotification(int notificationID)
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    Notification exist = await _warnRepo.GetNotificationByID(notificationID);
                    exist.UpdatedTime = DateTime.Now;
                    exist.Status = 1;
                    var data = await _warnRepo.UpdateNotification(exist);

                    // 清楚redis
                    string prefix = RedisKeyPrefix.Notification;
                    // key为设备id_通知类型,value 为 此通知在通知表中的id
                    _cache.Remove(prefix + exist.EqpID + "_" + exist.NotificationType);

                    // 发送通知
                    MssEventMsg msg = new MssEventMsg();
                    msg.msg = "off";
                    msg.type = MssEventType.Notification;
                    msg.eqp = new Equipment()
                    {
                        ID = exist.EqpID
                    };
                    _queues.AlarmQueue.Enqueue(msg);

                    ret.code = Code.Success;
                    ret.data = data;

                    scope.Complete();
                }
                
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }
    }
}
