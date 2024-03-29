﻿using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSS.API.Core.Job
{
    using IOCContainer = IServiceProvider;
    public class QuartzStart
    {
        private readonly IOCContainer _iocContainer;
        private readonly IConfiguration _configuration;
        public QuartzStart(IOCContainer iocContainer, IConfiguration configuration)
        {
            _iocContainer = iocContainer;
            _configuration = configuration;
        }
        public async Task Start()
        {
            try
            {
                // Grab the Scheduler instance from the Factory                  
                //NameValueCollection props = new NameValueCollection
                //{
                //    { "quartz.serializer.type", "binary" }
                //};
                //StdSchedulerFactory factory = new StdSchedulerFactory(props);
                IScheduler scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                scheduler.JobFactory = new IOCJobFactory(_iocContainer);
                // 定义一个 Job                  
                IJobDetail job = JobBuilder.Create<HealthJob>()
                    .WithIdentity("HealthJob", "group1")
                    .Build();
                ITrigger trigger = TriggerBuilder.Create()
                    .WithCronSchedule(_configuration["HealthJob"])//每天0点执行一次
                    //.WithSimpleSchedule(x => x.WithIntervalInSeconds(1).RepeatForever())//每秒执行一次
                    .WithIdentity("HealthTrigger", "group1") // 给任务一个名字 
                    .StartNow()
                    //.StartAt(DateTime.Now) // 设置任务开始时间                      
                    .ForJob("HealthJob", "group1") //给任务指定一个分组                      
                                                   //.WithSimpleSchedule(x => x                    
                                                   //.WithIntervalInSeconds(1)  //循环的时间 1秒1次                     
                                                   //.RepeatForever())                    
                    .Build();                  // 等待执行任务                  
                await scheduler.ScheduleJob(job, trigger);
                // some sleep to show what's happening                  
                //await Task.Delay(TimeSpan.FromMilliseconds(2000));              
                // 启动任务调度器                  
                await scheduler.Start();
            }
            catch (SchedulerException se)
            {
                await Console.Error.WriteLineAsync(se.ToString());
            }
        }
        public void Stop()
        {
        }
    }
    public class IOCJobFactory : IJobFactory
    {

        protected readonly IOCContainer Container;

        public IOCJobFactory(IOCContainer container)

        {

            Container = container;

        }

        //Called by the scheduler at the time of the trigger firing, in order to produce

        //a Quartz.IJob instance on which to call Execute.

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)

        {

            return Container.GetService(bundle.JobDetail.JobType) as IJob;

        }

        // Allows the job factory to destroy/cleanup the job if needed.

        public void ReturnJob(IJob job)

        {

        }

    }

}
