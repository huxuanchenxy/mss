using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using MSS.API.Core.Common;
using MSS.API.Model.DTO;

using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MSS.API.Common;
using MSS.Common.Consul;
using MSS.API.Common.Utility;
namespace MSS.API.Core.V1.Business
{
    public class OrgService: IOrgService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IOrgRepo<OrgTree> _orgRepo;
        private readonly IServiceDiscoveryProvider _consulServiceProvider;

        public OrgService(IOrgRepo<OrgTree> orgRepo, IServiceDiscoveryProvider consulServiceProvider)
        {
            //_logger = logger;
            _orgRepo = orgRepo;
            _consulServiceProvider = consulServiceProvider;
        }

        // 获取所有顶级节点下所有用户，包括子级节点的用户
        public async Task<ApiResult> ListTopNodeWithUsers()
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<OrgTree> nodes_all = await _orgRepo.ListAllOrgNode();
                List<OrgUser> users = await _orgRepo.ListAllOrgUser();
                Dictionary<int, OrgUserView> orgs = new Dictionary<int, OrgUserView>();
                foreach (OrgUser user in users)
                {
                    OrgTree topNode = _findTopNode(user.NodeID, nodes_all);
                    if (topNode != null)
                    {
                        if (orgs.ContainsKey(topNode.ID))
                        {
                            orgs[topNode.ID].UserIDs.Add(user.UserID);
                        }
                        else
                        {
                            OrgUserView org = new OrgUserView();
                            org.ID = topNode.ID;
                            org.UserIDs = new List<int>();
                            org.UserIDs.Add(user.UserID);
                            orgs.Add(org.ID, org);
                        }
                    }
                }
                List<OrgUserView> data = orgs.Select(c => c.Value).ToList();

                ret.code = Code.Success;
                ret.data = data;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            return ret;
        }

        private OrgTree _findTopNode(int nodeId, List<OrgTree> nodes)
        {
            OrgTree node = nodes.Where(c => c.ID == nodeId).First();
            if (node.ParentID == null)
            {
                return node;
            }
            else
            {
                return _findTopNode((int)node.ParentID, nodes);
            }
        }
    }
}
