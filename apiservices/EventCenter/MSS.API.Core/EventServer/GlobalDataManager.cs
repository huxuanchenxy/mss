using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MSS.API.Model.Data;
using Microsoft.Extensions.Caching.Distributed;
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
using MSS.API.Model.DTO;
using MSS.API.Dao.Interface;
using MSS.API.Core.V1.Business;
using System.Threading;
namespace MSS.API.Core.EventServer
{
    public class GlobalDataManager : IJob
    {
        private readonly ILogger _logger;
        private readonly IDistributedCache _cache;
        private readonly IOrgService _orgService;
        private readonly IWarnningSettingRepo<EarlyWarnningSetting> _warnSettingRepo;
        private readonly IGlobalDataManagerRepo _globalRepo;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;
        private List<EarlyWarnningSetting> _settings;
        private Dictionary<string, double?> _exValues = new Dictionary<string, double?>();
        
        // 预警规则中牵涉到的点位列表
        private Dictionary<string, PidTable> _pids = new Dictionary<string, PidTable>();

        private List<Equipment> _allEqp = new List<Equipment>();
        private EquipmentConfig _EqpConfig;

        // 事件队列
        private BufferBlock<UpdateEventType> _updateEventsBuffer = new BufferBlock<UpdateEventType>();
        
        // 
        AutoResetEvent jobDoneEvent = new AutoResetEvent(false);
        public GlobalDataManager(ILogger<MsgQueueWatcher> logger, IOrgRepo<OrgTree> orgRepo,
            IDistributedCache cache, IOrgService orgService,
            IWarnningSettingRepo<EarlyWarnningSetting> warnSettingRepo,
            IServiceDiscoveryProvider consulServiceProvider, IGlobalDataManagerRepo globalRepo)
        {
            _logger = logger;
            _cache = cache;
            _orgService = orgService;
            _warnSettingRepo = warnSettingRepo;
            _consulServiceProvider = consulServiceProvider;
            _globalRepo = globalRepo;
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
        public Dictionary<string, PidTable> AllPID
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

        public void WaitAllPidTask() {
            jobDoneEvent.WaitOne();
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
                        case UpdateEventType.InitAllPid:
                            await this.initAllPid();
                            break;
                        case UpdateEventType.InitStatisticsDimention:
                            await this.initStatisticsDimention();
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
            var services = await _consulServiceProvider.GetServiceAsync("OrgService");
            string url = services + "/api/v1/orguser/usersintoporg";
            ApiResult result = HttpClientHelper.GetResponse<ApiResult>(url);

            string prefix = RedisKeyPrefix.Org;
            if (result.code == Code.Success)
            {
                List<OrgUserView> orgs = JsonConvert.DeserializeObject<List<OrgUserView>>(result.data.ToString());
                foreach (OrgUserView org in orgs)
                {
                    _cache.SetString(prefix + org.ID, string.Join(",", org.UserIDs));
                }
            }
        }

        public async Task initWarnSetting()
        {
            _settings = await _warnSettingRepo.ListAllWarnningSetting();
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
            
        }

        public async Task initAllPid()
        {
            // 查询设备类型和参数涵盖的pid
            List<PidTable> pids = await _warnSettingRepo.ListAllPid();
            foreach (PidTable pid in pids)
            {
                if (!_pids.ContainsKey(pid.pid))
                {
                    _pids.Add(pid.pid, pid);
                }
            }
            jobDoneEvent.Set();
        }

        public async Task initStatisticsDimention()
        {
            List<StatisticsDimension> curDims = await _globalRepo.ListStatisticsDimension(); 
            List<StatisticsDimension> list = await _globalRepo.ListEqpDimension();
            List<AllArea> laa = await _globalRepo.GetAllArea();
            foreach (StatisticsDimension dim in list)
            {
                int[] path = dim.LocationPath.Split(',').Select(int.Parse).ToArray();
                for (int i = 0; i < path.Length; ++i)
                {
                    var area = laa.Where(a => a.Tablename == i && a.ID == path[i]).FirstOrDefault();
                    if (area != null)
                    {
                        switch (i)
                        {
                            case 0:
                                dim.LocationLevel1 = area.ID;
                                dim.LocationLevel1Name = area.AreaName;
                                break;
                            case 1:
                                dim.LocationLevel2 = area.ID;
                                dim.LocationLevel2Name = area.AreaName;
                                break;
                            case 2:
                                dim.LocationLevel3 = area.ID;
                                dim.LocationLevel3Name = area.AreaName;
                                break;
                        }
                    }
                }

                StatisticsDimension exist = curDims.Where(c => c.EqpID == dim.EqpID).FirstOrDefault();
                if (exist == null)
                {
                    await _globalRepo.SaveStatisticsDimension(new List<StatisticsDimension>{dim});
                }
            }
        }

        public void postUpdateEvent(UpdateEventType type)
        {
            _updateEventsBuffer.Post(type);
            
        }
    }
}