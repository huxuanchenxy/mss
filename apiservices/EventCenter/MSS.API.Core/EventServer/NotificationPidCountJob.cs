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
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
namespace MSS.API.Core.EventServer
{
    public class NotificationPidCountJob : IJob
    {
        private readonly ILogger _logger;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        private readonly GlobalDataManager _globalDataManager;
        private readonly IWarnningRepo<EarlyWarnning> _warn;
        private readonly IDistributedCache _cache;
        private readonly EventQueues _queues;
        private readonly IConfiguration _config;
        private readonly IOrgRepo<OrgTree> _orgRepo;
        public NotificationPidCountJob(ILogger<WarningJob> logger, GlobalDataManager globalDataManager,
            IServiceDiscoveryProvider consulServiceProvider, IDistributedCache cache,
            IWarnningRepo<EarlyWarnning> warn, EventQueues queues, IConfiguration config,
            IOrgRepo<OrgTree> orgRepo)
        {
            _logger = logger;
            _globalDataManager = globalDataManager;
            _consulServiceProvider = consulServiceProvider;
            _cache = cache;
            _warn = warn;
            _queues = queues;
            _config = config;
            _orgRepo = orgRepo;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            while(true)
            {
                try
                {
                    //取status=0未处理的通知
                    var _services = await _consulServiceProvider.GetServiceAsync("EqpService");
                    var strRet = HttpClientHelper.GetResponse(_services + "/api/v1/NotificationPidcount/GetPageList");
                    if (!string.IsNullOrEmpty(strRet))
                    {
                        ApiResult objret = JsonConvert.DeserializeObject<ApiResult>(strRet.ToString());
                        if (objret != null && objret.data != null)
                        {
                            try
                            {
                                NotificationPidcountPageView dataobj = JsonConvert.DeserializeObject<NotificationPidcountPageView>(objret.data.ToString());
                                if (dataobj.total > 0)
                                {
                                    _sendNotification("点位超过容量预设值", dataobj.total);
                                }
                            }
                            catch (Exception ex) { }
                            
                        }
                        
                    }
                    //Thread.Sleep(30000);
                    Thread.Sleep(10000);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.StackTrace);
                }
            }
        }

        public class NotificationPidcountPageView
        {
            public int total { get; set; }
        }

        private void _sendNotification(string content,int count)
        {
            MssEventMsg msg = new MssEventMsg();
            msg.msg = content;
            msg.type = MssEventType.NotificationPidCount;
            msg.eqp = new Equipment()
            {
                ID = -1
            };
            msg.detail = count;
            _queues.AlarmQueue.Enqueue(msg);
        }

    }
}