using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using Microsoft.Extensions.Caching.Distributed;
using MSS.API.Core.V1.Business;
using MSS.API.Common;
using MSS.API.Core.Common;
using MSS.API.Core.Models.Ex;
using System.Threading.Tasks;
using MSS.Common.Consul;
using MSS.API.Common.Utility;
using Quartz;
using System.Threading.Tasks.Dataflow;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace MSS.API.Core.EventServer
{
    public class GlobalDataManager : IJob
    {
        private readonly ILogger _logger;
        private readonly IOrgRepo<OrgTree> _orgRepo;
        private readonly IDistributedCache _cache;
        private readonly IOrgService _orgService;
        private readonly IWarnningSettingRepo<EarlyWarnningSetting> _warnSettingRepo;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        private List<EarlyWarnningSetting> _settings;
        private Dictionary<string, double?> _exValues = new Dictionary<string, double?>();
        
        // 预警规则中牵涉到的点位列表
        private List<PidTable> _pids = new List<PidTable>();

        private List<Equipment> _allEqp = new List<Equipment>();
        private EquipmentConfig _EqpConfig;

        // 事件队列
        private BufferBlock<UpdateEventType> _updateEventsBuffer = new BufferBlock<UpdateEventType>();
        public GlobalDataManager(ILogger<MsgQueueWatcher> logger, IOrgRepo<OrgTree> orgRepo,
            IDistributedCache cache, IOrgService orgService,
            IWarnningSettingRepo<EarlyWarnningSetting> warnSettingRepo,
            IServiceDiscoveryProvider consulServiceProvider)
        {
            _logger = logger;
            _orgRepo = orgRepo;
            _cache = cache;
            _orgService = orgService;
            _warnSettingRepo = warnSettingRepo;
            _consulServiceProvider = consulServiceProvider;
        }

        public EquipmentConfig EqpConfig
        {
            get
            {
                return _EqpConfig;
            }
        }

        public List<Equipment> AllEqp
        {
            get
            {
                return _allEqp;
            }
        }
        public List<PidTable> AllPID
        {
            get
            {
                return _pids;
            }
        }

        public List<EarlyWarnningSetting> Rules
        {
            get
            {
                return _settings;
            }
        }

        public Dictionary<string, double?> ExRulesValue
        {
            get
            {
                return _exValues;
            }
        }

        public async Task Execute(IJobExecutionContext context)
        {
            while (true)
            {
                try
                {
                    // int curid = Thread.CurrentThread.ManagedThreadId;
                    UpdateEventType type = _updateEventsBuffer.Receive();
                    switch (type)
                    {
                        case UpdateEventType.InitEqpConfig:
                            await this.initEquipmentConfig();
                            break;
                        case UpdateEventType.InitEquipment:
                            await this.initEquipment();
                            break;
                        case UpdateEventType.InitTopOrg:
                            await this.initTopOrg();
                            break;
                        case UpdateEventType.InitWarnSetting:
                            await this.initWarnSetting();
                            break;
                        default:
                            _logger.LogError("未知的更新事件：" + type);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.StackTrace);
                }
            }
        }

        public async Task initEquipmentConfig()
        {
            var _services = await _consulServiceProvider.GetServiceAsync("EqpService");
            IHttpClientHelper<ApiResult> httpHelper = new HttpClientHelper<ApiResult>();
            ApiResult result = await httpHelper.
                GetSingleItemRequest(_services + "/api/v1/EquipmentConfig/1");
            if (result.code == Code.Success)
            {
                _EqpConfig = JsonConvert.DeserializeObject<EquipmentConfig>(result.data.ToString());
            }
        }

        // 所有设备及对应TopOrg节点存入redis
        public async Task initEquipment()
        {
            string prefix = RedisKeyPrefix.Eqp;
            _allEqp = await _warnSettingRepo.ListAllEquipment();
            foreach (Equipment eqp in _allEqp)
            {
                _cache.SetString(prefix + eqp.ID, eqp.TopOrg.ToString());
            }
        }

        // 初始化所有顶级节点（公司）下所有用户，包括子节点
        public async Task initTopOrg()
        {
            string prefix = RedisKeyPrefix.Org;
            ApiResult result = await _orgService.ListTopNodeWithUsers();
            if (result.code == Code.Success)
            {
                List<OrgUserView> orgs = result.data as List<OrgUserView>;
                foreach (OrgUserView org in orgs)
                {
                    _cache.SetString(prefix + org.ID, string.Join(",", org.UserIDs));
                }
            }
        }

        public async Task initWarnSetting()
        {
            _settings = await _warnSettingRepo.ListWarnningSettingByPage(
                null, null, null, null, null, null);
            HashSet<int> eqpTypeList = new HashSet<int>();
            HashSet<string> propList = new HashSet<string>();
            foreach (EarlyWarnningSetting item in _settings)
            {
                if (!eqpTypeList.Contains(item.EquipmentTypeID))
                {
                    eqpTypeList.Add(item.EquipmentTypeID);
                }
                if (!propList.Contains(item.ParamID))
                {
                    propList.Add(item.ParamID);
                }
                foreach (EarlyWarnningSettingEx settingex in item.SettingEx)
                {
                    if (!_exValues.ContainsKey(settingex.pid))
                    {
                        _exValues.Add(settingex.pid, null);
                    }
                }
            }
            // 查询设备类型和参数涵盖的pid
            _pids = await _warnSettingRepo.ListPidTable(new List<int>(eqpTypeList),
                new List<string>(propList));
        }

        public void postUpdateEvent(UpdateEventType type)
        {
            _updateEventsBuffer.Post(type);
        }
    }
}