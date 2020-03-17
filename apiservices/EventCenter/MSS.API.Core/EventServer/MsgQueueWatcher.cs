using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using MSS.API.Core.Common;
using System.Threading;
namespace MSS.API.Core.EventServer
{
    public class MsgQueueWatcher : IJob
    {
        private readonly ILogger _logger;
        private readonly EventQueues _queues;
        private readonly IDistributedCache _cache;
        public IHubContext<MssEventHub, IMssEventClient> _eventHubContext { get; }
        public MsgQueueWatcher(ILogger<MsgQueueWatcher> logger, IDistributedCache cache,
            IHubContext<MssEventHub, IMssEventClient> eventHubContext, EventQueues queues)
        {
            _logger = logger;
            _eventHubContext = eventHubContext;
            _queues = queues;
            _cache = cache;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            while (true)
            {
                try
                {
                    MssEventMsg msg;
                    bool ret = _queues.AlarmQueue.TryDequeue(out msg);
                    if (ret)
                    {
                        if (msg.type != MssEventType.NotificationPidCount)
                        {
                            string value = _cache.GetString(RedisKeyPrefix.Eqp + msg.eqp.ID);
                            if (value != null)
                            {
                                int topOrg = int.Parse(value);
                                string users_str = _cache.GetString(RedisKeyPrefix.Org + topOrg);
                                if (users_str != null)
                                {
                                    string[] idstrs = users_str.Split(',');
                                    await _eventHubContext.Clients.Users(idstrs).RecieveMsg(msg);
                                }
                                // 发送管理员组
                                await _eventHubContext.Clients.Groups(RedisKeyPrefix.SuperGroup)
                                    .RecieveMsg(msg);
                            }
                        }
                        else
                        {
                            // 发送管理员组
                            await _eventHubContext.Clients.Groups(RedisKeyPrefix.SuperGroup)
                                .RecieveMsg(msg);
                        }
                        
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.StackTrace);
                }
                Thread.Sleep(1000);
            }            
        }
    }
}