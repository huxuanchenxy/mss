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

        
    }
}
