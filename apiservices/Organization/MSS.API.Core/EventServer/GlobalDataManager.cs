using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using MSS.API.Model.DTO;
using Microsoft.Extensions.Caching.Distributed;
using MSS.API.Core.V1.Business;
using MSS.API.Common;
using MSS.API.Core.Common;
using System.Threading.Tasks;
namespace MSS.API.Core.EventServer
{
    public class GlobalDataManager
    {
        private readonly ILogger _logger;
        private readonly IOrgRepo<OrgTree> _orgRepo;
        private readonly IDistributedCache _cache;
        private readonly IOrgService _orgService;
        public GlobalDataManager(ILogger<MsgQueueWatcher> logger, IOrgRepo<OrgTree> orgRepo,
            IDistributedCache cache, IOrgService orgService)
        {
            _logger = logger;
            _orgRepo = orgRepo;
            _cache = cache;
            _orgService = orgService;
        }

        public async Task initEquipment()
        {
            string prefix = RedisKeyPrefix.Eqp;
            List<Equipment> list = await _orgRepo.ListAllEquipment();
            foreach (Equipment eqp in list)
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
    }
}