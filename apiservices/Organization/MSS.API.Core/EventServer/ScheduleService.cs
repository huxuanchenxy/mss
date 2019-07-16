
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Quartz;
using Quartz.Impl;
namespace MSS.API.Core.EventServer
{
    internal class ScheduleService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;
        public ScheduleService(ILogger<ScheduleService> logger)
        {
            _logger = logger;
            NameValueCollection props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            _schedulerFactory = new StdSchedulerFactory(props);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("定时服务启动.");
            _scheduler = await _schedulerFactory.GetScheduler();
            await _scheduler.Start();
            //3、创建一个触发器
            // var trigger = TriggerBuilder.Create()
            //     .WithSimpleSchedule(x => x.WithIntervalInSeconds(2).RepeatForever())//每两秒执行一次
            //     .Build();
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(1)
                    .RepeatForever())
                .Build();
            //4、创建任务
            var jobDetail = JobBuilder.Create<AlarmDataJob>()
                .WithIdentity("job", "group")
                .Build();
            //5、将触发器和任务器绑定到调度器中
            await _scheduler.ScheduleJob(jobDetail, trigger);

            
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
