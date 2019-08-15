using Microsoft.Extensions.DependencyInjection;
using System;
using Quartz;
using Quartz.Spi;
namespace MSS.API.Core.EventServer
{
    public class MssJobFactory : IJobFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public MssJobFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            // return _serviceProvider.GetService<MsgQueueWatcher>();
            return (IJob)_serviceProvider.GetService(bundle.JobDetail.JobType);
        }

        public void ReturnJob(IJob job)
        {
            var disposable = job as IDisposable;
            disposable?.Dispose();
        }
    }
}