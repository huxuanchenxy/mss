using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;
using MSS.Common.Consul;
using Newtonsoft.Json;
using MSS.API.Model.Data;
using MSS.API.Core.Models.Ex;
using Microsoft.Extensions.Caching.Distributed;
using MSS.API.Core.Common;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using MSS.API.Dao.Interface;
namespace MSS.API.Core.EventServer
{
    public class AlarmStatisticJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        private readonly GlobalDataManager _globalDataManager;
        private readonly IStatisticsRepo _statisticsRepo;
        private readonly IDistributedCache _cache;
        private readonly EventQueues _queues;
        private readonly IConfiguration _config;
        public AlarmStatisticJob(ILogger<AlarmJob> logger, GlobalDataManager globalDataManager,
            IServiceDiscoveryProvider consulServiceProvider, IDistributedCache cache,
            EventQueues queues, IConfiguration config, IStatisticsRepo statisticsRepo)
        {
            _logger = logger;
            _globalDataManager = globalDataManager;
            _consulServiceProvider = consulServiceProvider;
            _cache = cache;
            _queues = queues;
            _config = config;
            _statisticsRepo = statisticsRepo;
        }
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                string server = _config["Kafka:Servers"];
                string topic = "event";
                var conf = new ConsumerConfig
                {
                    GroupId = "consumer-event-group-for-statistic",
                    BootstrapServers = server,
                    AutoOffsetReset = AutoOffsetReset.Earliest,
                    EnableAutoCommit = false
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
                                _logger.LogDebug($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                                string msg = cr.Value;
                                Alarm alarm = JsonConvert.DeserializeObject<Alarm>(cr.Value);
                                if (alarm.Ack != 0) {
                                    _logger.LogDebug($"Consumed message '{cr.Value}' at: '{cr.TopicPartitionOffset}'.");
                                }
                                PidTable pidInfo = null;
                                if (_globalDataManager.AllPID.ContainsKey(alarm.pid))
                                {
                                    pidInfo = _globalDataManager.AllPID[alarm.pid];
                                }
                                
                                if (pidInfo != null && pidInfo.EqpID != null)
                                {
                                    // redis中查找看是否有报警存在
                                    string prefix = RedisKeyPrefix.AlarmStatistic;
                                    string value = _cache.GetString(prefix + alarm.pid);
                                    
                                    if (value == null && alarm.Ack == 0)
                                    {
                                        // 插入redis
                                        _cache.SetString(prefix + alarm.pid, cr.Value);
                                    }
                                    else if (value != null && alarm.Ack != 0)
                                    {
                                        _cache.Remove(prefix + alarm.pid);
                                        Alarm start = JsonConvert.DeserializeObject<Alarm>(value);
                                        _genRecord(start, alarm, (int)pidInfo.EqpID);
                                    }
                                }

                                c.Commit();
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

        private void _genRecord(Alarm start, Alarm end, int eqpID)
        {
            StatisticsAlarm alarmRecord = new StatisticsAlarm();
            alarmRecord.EqpID = eqpID;
            alarmRecord.Level = start.eLevel;
            alarmRecord.OccurTime = start.ETime;
            alarmRecord.RecoverTime = end.ETime;
            alarmRecord.Prop = start.PidDes;
            alarmRecord.Content = start.Des;
            int elapsedtime = (int)(end.ETime - start.ETime).TotalMilliseconds;
            alarmRecord.ElapsedTime = elapsedtime > 0 ? elapsedtime : 0;
            if (alarmRecord.ElapsedTime > 0)
            {
                _statisticsRepo.SaveStatisticsAlarm(alarmRecord);
            }
        }
    }
}