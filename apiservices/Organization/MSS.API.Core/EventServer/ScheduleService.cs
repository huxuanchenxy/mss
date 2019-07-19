
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
            await _globalDataManager.initEquipment();
            await _globalDataManager.initTopOrg();

            _logger.LogInformation("定时服务启动.");
            _scheduler = await _schedulerFactory.GetScheduler();
            _scheduler.JobFactory = _mssJobFactory;
            await _scheduler.Start();

            // 消息队列监听
            IJobDetail job = JobBuilder.Create<MsgQueueWatcher>()
                .WithIdentity("myJob", "group1")
                .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithInterval(TimeSpan.FromTicks(1000000)) //1秒 = 10000000 ticks
                    .RepeatForever())
                .Build();
            await _scheduler.ScheduleJob(job, trigger);

            
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
