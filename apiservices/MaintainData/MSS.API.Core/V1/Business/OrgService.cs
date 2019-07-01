using MSS.API.Dao.Interface;
using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using MSS.API.Core.Common;
using MSS.API.Model.DTO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MSS.API.Common;

namespace MSS.API.Core.V1.Business
{
    public class OrgService: IOrgService
    {
        //private readonly ILogger<UserService> _logger;
        private readonly IOrgRepo<OrgTree> _orgRepo;

        public OrgService(IOrgRepo<OrgTree> orgRepo)
        {
            //_logger = logger;
            _orgRepo = orgRepo;
        }

        public async Task<DataResult> GetAllOrg()
        {
            DataResult ret = new DataResult();
            try
            {
                List<OrgTree> nodes_all = await _orgRepo.ListAllOrgNode();
                // 获取顶级节点
                List<OrgTree> nodes_org = nodes_all.Where(c => c.ParentID == null).ToList();
                // 解析单个组织
                List<object> orgs = new List<object>();
                foreach (OrgTree node in nodes_org)
                {
                    if (!node.IsDel)
                    {
                        var obj = _parseOrgTree(node, nodes_all);
                        orgs.Add(obj);
                    }
                }
                ret.Result = RESULT.OK;
                ret.Data = orgs;
            }
            catch (Exception ex)
            {
                ret.Result = RESULT.FAIL;
                ret.Message = ex.Message;
            }

            return ret;
        }

        public async Task<DataResult> GetOrgByIDs(List<int> ids)
        {
            DataResult ret = new DataResult();
            try
            {
                List<OrgTree> nodes_all = await _orgRepo.ListAllOrgNode();
                // 获取顶级节点
                List<OrgTree> nodes_org = nodes_all.Where(c => ids.Contains(c.ID)).ToList();
                // 解析单个组织
                List<object> orgs = new List<object>();
                foreach (OrgTree node in nodes_org)
                {
                    if (!node.IsDel)
                    {
                        var obj = _parseOrgTree(node, nodes_all);
                        orgs.Add(obj);
                    }
                }
                ret.Result = RESULT.OK;
                ret.Data = orgs;
            }
            catch (Exception ex)
            {
                ret.Result = RESULT.FAIL;
                ret.Message = ex.Message;
            }

            return ret;
        }

        public async Task<DataResult> GetOrgByUserID(int userId)
        {
            DataResult ret = new DataResult();
            try
            {
                
                OrgUser orguser = await _orgRepo.GetOrgUserByUserID(userId);
                if (orguser != null) {
                    ret = await GetOrgByIDs(new List<int> { orguser.NodeID });
                } else {
                    ret = await GetAllOrg();
                }
                // List<OrgTree> nodes_all = await _orgRepo.ListAllOrgNode();
                // // 找到用户节点属于的顶级节点
                // OrgTree topNode = _findTopNode(orguser.NodeID, nodes_all);
                // // 解析单个组织
                // List<object> orgs = new List<object>();
                // if (!topNode.IsDel) {
                //     var obj = _parseOrgTree(topNode, nodes_all);
                //     orgs.Add(obj);
                // }
                // ret.Result = RESULT.OK;
                // ret.Data = orgs;
            }
            catch (Exception ex)
            {
                ret.Result = RESULT.FAIL;
                ret.Message = ex.Message;
            }

            return ret;
        }

        private OrgTree _findTopNode(int nodeId, List<OrgTree> nodes)
        {
            OrgTree node = nodes.Where(c => c.ID == nodeId).First();
            if (node.ParentID == null)
            {
                return node;
            } else {
                return _findTopNode((int)node.ParentID, nodes);
            }
        }

        private object _parseOrgTree(OrgTree parentNode, List<OrgTree> nodes)
        {
            var node_p = new
            {
                id = parentNode.ID,
                label = parentNode.Name,
                node_type = parentNode.NodeType,
                children = new List<object>()
            };
            List<OrgTree> nodes_children = nodes.Where(c => c.ParentID == parentNode.ID).ToList();
            if (nodes_children.Count > 0)
            {
                foreach (OrgTree child in nodes_children)
                {
                    if (!child.IsDel)
                    {
                        var node_c = _parseOrgTree(child, nodes);
                        node_p.children.Add(node_c);
                    }
                }
            }
            return node_p;
        }

        public async Task<DataResult> AddOrgNode(OrgTree node)
        {
            DataResult ret = new DataResult();
            try {
                using (TransactionScope scope = new TransactionScope())
                {
                    bool isExist = await _orgRepo.CheckNodeExist(node);
                    if (!isExist)
                    {
                        var data = await _orgRepo.SaveOrgNode(node);

                        //保存扩展属性
                        if (node.PropEx != null && node.PropEx.Count > 0)
                        {
                            bool propSavedOk = await _saveNodeProperty(data);
                            if (!propSavedOk)
                            {
                                throw new Exception("存储节点属性失败");
                            }
                        }
                        
                        ret.Result = RESULT.OK;
                        ret.Data = data;
                    }
                    else
                    {
                        ret.Result = RESULT.REINSERT;
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.Result = RESULT.FAIL;
                ret.Message = ex.Message;
            }
            
            return ret;
        }

        public async Task<DataResult> UpdateOrgNode(OrgTree node)
        {
            DataResult ret = new DataResult();
            try {
                using (TransactionScope scope = new TransactionScope())
                {
                    bool isExist = await _orgRepo.CheckNodeExist(node);
                    if (!isExist)
                    {
                        var data = await _orgRepo.UpdateOrgNode(node);
                        //由于节点类型有可能更新，如果更新则节点对应的扩展属性会有不同，
                        //为了逻辑同一，对属性的更新都先删除再添加
                        //删除已有属性
                        await _orgRepo.DeleteOrgNodeProperty(node);
                        //保存扩展属性
                        if (node.PropEx.Count > 0)
                        {
                            bool propSavedOk = await _saveNodeProperty(data);
                            if (!propSavedOk)
                            {
                                throw new Exception("存储节点属性失败");
                            }
                        }
                        ret.Result = RESULT.OK;
                        ret.Data = data;
                    }
                    else
                    {
                        ret.Result = RESULT.REINSERT;
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.Result = RESULT.FAIL;
                ret.Message = ex.Message;
            }
            
            return ret;
        }

        private async Task<bool> _saveNodeProperty (OrgTree node)
        {
            //获取此节点类型对应的属性
            List<OrgNodeTypeProperty> typeProps = await _orgRepo.ListNodeTypeProperty(node.NodeType);
            List<OrgNodeProperty> props = new List<OrgNodeProperty>();
            foreach(OrgNodeProperty prop in node.PropEx)
            {
                foreach (var tp in typeProps)
                {
                    // 只存数据库内配置的属性
                    if (tp.NodeAttr.Equals(prop.NodeAttr))
                    {
                        OrgNodeProperty newprop = new OrgNodeProperty();
                        newprop.NodeID = node.ID;
                        newprop.NodeAttr = prop.NodeAttr;
                        newprop.AttrValue = prop.AttrValue;
                        props.Add(prop);
                        break;
                    }
                }
                prop.NodeID = node.ID;
            }
            if (props.Count > 0)
            {
                await _orgRepo.SaveOrgNodeProperty(props);
                return true;
            }
            return false;
        }

        public async Task<DataResult> DeleteOrgNode(OrgTree node)
        {
            DataResult ret = new DataResult();
            try
            {
                OrgTree exist = await _orgRepo.GetNode(node.ID);
                if (exist == null)
                {
                    ret.Result = RESULT.NOTFOUNT;
                    return ret;
                }
                await _orgRepo.DeleteOrgNode(node);
                await _orgRepo.UnbindOrgNodeUsers(node);
                ret.Result = RESULT.OK;
            }
            catch (Exception ex)
            {
                ret.Result = RESULT.FAIL;
                ret.Message = ex.Message;
            }
            
            return ret;
        }

        public async Task<DataResult> BindOrgNodeUsers(OrgUserView nodeView) {
            DataResult ret = new DataResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    //取消bind当前的用户
                    OrgTree node = new OrgTree();
                    node.ID = nodeView.ID;
                    node.UpdatedBy = nodeView.CreatedBy;
                    node.UpdatedTime = nodeView.CreatedTime;
                    await _orgRepo.UnbindOrgNodeUsers(node);
                    //bind新用户
                    List<OrgUser> users = new List<OrgUser>();
                    foreach (int id in nodeView.UserIDs)
                    {
                        OrgUser user = new OrgUser();
                        user.UserID = id;
                        user.NodeID = nodeView.ID;
                        user.CreatedBy = nodeView.CreatedBy;
                        user.CreatedTime = nodeView.CreatedTime;

                        users.Add(user);
                    }
                    await _orgRepo.BindOrgNodeUsers(users);

                    ret.Result = RESULT.OK;

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.Result = RESULT.FAIL;
                ret.Message = ex.Message;
            }

            return ret;
        }

        public async Task<DataResult> GetOrgNodeUsers(int id)
        {
            DataResult ret = new DataResult();
            try
            {
                // 获取此节点已选择的用户
                List<OrgUser> users = await _orgRepo.ListOrgNodeUsers(id);

                // 获取此节点之外已选择的用户，此节点不可再选
                List<OrgUser> usersNotThisNode = await _orgRepo.ListUnOrgNodeUsers(id);
                
                ret.Result = RESULT.OK;
                ret.Data = new {
                    users = users,
                    disabledUsers = usersNotThisNode
                };
            }
            catch (Exception ex)
            {
                ret.Result = RESULT.FAIL;
                ret.Message = ex.Message;
            }

            return ret;
        }

        public async Task<DataResult> GetCanSelectedUsers(int id)
        {
            DataResult ret = new DataResult();
            try
            {
                List<User> users = await _orgRepo.ListUsersNotThisNode(id);

                ret.Result = RESULT.OK;
                ret.Data = users;
            }
            catch (Exception ex)
            {
                ret.Result = RESULT.FAIL;
                ret.Message = ex.Message;
            }

            return ret;
        }

        public async Task<DataResult> GetNodeType() {
            DataResult ret = new DataResult();
            try
            {
                List<OrgNodeType> type = await _orgRepo.ListNodeType();

                ret.Result = RESULT.OK;
                ret.Data = type;
            }
            catch (Exception ex)
            {
                ret.Result = RESULT.FAIL;
                ret.Message = ex.Message;
            }

            return ret;
        }

        public async Task<DataResult> GetOrgNode(int id)
        {
            DataResult ret = new DataResult();
            try
            {
                OrgTree node = await _orgRepo.GetNodeView(id);

                ret.Result = RESULT.OK;
                ret.Data = node;
            }
            catch (Exception ex)
            {
                ret.Result = RESULT.FAIL;
                ret.Message = ex.Message;
            }

            return ret;
        }
    }
}
