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
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
namespace MSS.API.Core.EventServer
{
    public class AlarmJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        private readonly GlobalDataManager _globalDataManager;
        private readonly IDistributedCache _cache;
        private readonly EventQueues _queues;
        private readonly IConfiguration _config;
        public AlarmJob(ILogger<AlarmJob> logger, GlobalDataManager globalDataManager,
            IServiceDiscoveryProvider consulServiceProvider, IDistributedCache cache,
            EventQueues queues, IConfiguration config)
        {
            _logger = logger;
            _globalDataManager = globalDataManager;
            _consulServiceProvider = consulServiceProvider;
            _cache = cache;
            _queues = queues;
            _config = config;
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                string server = _config["Kafka:Servers"];
                string topic = "event";
                var conf = new ConsumerConfig
                {
                    GroupId = "consumer-event-group",
                    BootstrapServers = server,
                    AutoOffsetReset = AutoOffsetReset.Latest
                };

                using (var c = new ConsumerBuilder<Ignore, string>(conf).Build())
                {
                    c.Subscribe(topic);
                    try
                    {
                        while (true)
                        {
                            try
                            {
                                var cr = c.Consume();
                                // Console.WriteLine($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                                _logger.LogDebug(cr.Value);
                                string msg = cr.Value;
                                Alarm alarm = JsonConvert.DeserializeObject<Alarm>(cr.Value);
                                PidTable pidInfo = null;
                                if (_globalDataManager.AllPID.ContainsKey(alarm.pid))
                                {
                                    pidInfo = _globalDataManager.AllPID[alarm.pid];
                                }
                                
                                if (pidInfo != null && pidInfo.EqpID != null)
                                {
                                    var detial = new {
                                        Alarm = alarm,
                                        PidInfo = pidInfo
                                    };
                                    // redis中查找看是否有报警存在
                                    string prefix = RedisKeyPrefix.Alarm;
                                    string warnID = _cache.GetString(prefix + alarm.pid);
                                    if (warnID == null && alarm.Ack == 0)
                                    {
                                        // 插入redis
                                        _cache.SetString(prefix + alarm.pid, "0");
                                        _sendAlarm((int)pidInfo.EqpID, "on", detial);
                                    }
                                    else if (warnID != null && alarm.Ack != 0)
                                    {
                                        _cache.Remove(prefix + alarm.pid);
                                        _sendAlarm((int)pidInfo.EqpID, "off", detial);
                                    }
                                }
                                
                            }
                            catch (ConsumeException e)
                            {
                                Console.WriteLine($"Error occured: {e.Error.Reason}");
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e.Message);
                                _logger.LogError(e.StackTrace);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        // Ensure the consumer leaves the group cleanly and final offsets are committed.
                        c.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
            }
            return Task.CompletedTask;
        }

        private void _sendAlarm(int eqpID, string content, object detail)
        {
            MssEventMsg msg = new MssEventMsg();
            msg.msg = content;
            msg.type = MssEventType.Alarm;
            msg.eqp = new Equipment()
            {
                ID = eqpID
            };
            msg.detail = detail;
            _queues.AlarmQueue.Enqueue(msg);
        }
    }
}