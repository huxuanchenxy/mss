
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
namespace MSS.API.Core.EventServer
{
    internal class ScheduleService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ISchedulerFactory _schedulerFactory;
        private readonly IJobFactory _mssJobFactory;
        private readonly GlobalDataManager _globalDataManager;
        private IScheduler _scheduler;
        public ScheduleService(ILogger<ScheduleService> logger, IJobFactory mssJobFactory, GlobalDataManager globalDataManager)
        {
            _logger = logger;
            _mssJobFactory = mssJobFactory;
            _globalDataManager = globalDataManager;
            NameValueCollection props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            _schedulerFactory = new StdSchedulerFactory(props);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //初始化设备列表及公司人员
            // await _globalDataManager.initEquipment();
            // await _globalDataManager.initTopOrg();
            _globalDataManager.postUpdateEvent(0);
            _globalDataManager.postUpdateEvent(1);
            _globalDataManager.postUpdateEvent(2);
            _globalDataManager.postUpdateEvent(3);

            _logger.LogInformation("定时服务启动.");
            _scheduler = await _schedulerFactory.GetScheduler();
            _scheduler.JobFactory = _mssJobFactory;
            await _scheduler.Start();

            // 全局数据更新job
            IJobDetail job_globalData = JobBuilder.Create<GlobalDataManager>()
                .Build();
            ITrigger trigger_globalData = TriggerBuilder.Create()
                .StartNow()
                .Build();
            await _scheduler.ScheduleJob(job_globalData, trigger_globalData);

            // 消息队列监听
            IJobDetail job = JobBuilder.Create<MsgQueueWatcher>()
                .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .StartNow()
                // .WithSimpleSchedule(x => x
                //     // .WithInterval(TimeSpan.FromTicks(10000000)) //1秒 = 10000000 ticks
                //     .WithIntervalInSeconds(1)
                //     .RepeatForever())
                .Build();
            await _scheduler.ScheduleJob(job, trigger);

            // 初始化配置，只执行一次
            // IJobDetail jobInit = JobBuilder.Create<InitConfigJob>()
            //     .Build();
            // ITrigger trigger_once = TriggerBuilder.Create()
            //     .StartNow()
            //     .Build();
            // await _scheduler.ScheduleJob(jobInit, trigger_once);

            // 预警分析任务
            IJobDetail job_warn = JobBuilder.Create<WarningJob>()
                .Build();
            ITrigger trigger_warn = TriggerBuilder.Create()
                .StartNow()
                .Build();
            await _scheduler.ScheduleJob(job_warn, trigger_warn);

            // 大中修寿命通知任务
            IJobDetail job_notification = JobBuilder.Create<NotificationJob>()
                .Build();
            ITrigger trigger_notification = TriggerBuilder.Create()
                .StartNow()
                .Build();
            await _scheduler.ScheduleJob(job_notification, trigger_notification);
            
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("定时服务结束.");

            // _timer?.Change(Timeout.Infinite, 0);
            _scheduler?.Shutdown();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // _timer?.Dispose();
            _scheduler?.Shutdown();
        }
    }
}
