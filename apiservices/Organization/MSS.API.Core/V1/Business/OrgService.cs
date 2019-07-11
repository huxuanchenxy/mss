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

        public async Task<ApiResult> GetAllOrg()
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<OrgNodeType> nodeTypes = await _orgRepo.ListNodeType();
                List<OrgTree> nodes_all = await _orgRepo.ListAllOrgNode();
                // 获取顶级节点
                List<OrgTree> nodes_org = nodes_all.Where(c => c.ParentID == null).ToList();
                // 解析单个组织
                List<object> orgs = new List<object>();
                foreach (OrgTree node in nodes_org)
                {
                    if (!node.IsDel)
                    {
                        var obj = _parseOrgTree(node, nodes_all, nodeTypes);
                        orgs.Add(obj);
                    }
                }
                ret.code = Code.Success;
                ret.data = orgs;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> GetOrgByIDs(List<int> ids)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<OrgNodeType> nodeTypes = await _orgRepo.ListNodeType();
                List<OrgTree> nodes_all = await _orgRepo.ListAllOrgNode();
                // 获取顶级节点
                List<OrgTree> nodes_org = nodes_all.Where(c => ids.Contains(c.ID)).ToList();
                // 解析单个组织
                List<object> orgs = new List<object>();
                foreach (OrgTree node in nodes_org)
                {
                    if (!node.IsDel)
                    {
                        var obj = _parseOrgTree(node, nodes_all, nodeTypes);
                        orgs.Add(obj);
                    }
                }
                ret.code = Code.Success;
                ret.data = orgs;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> GetOrgByUserID(int userId)
        {
            ApiResult ret = new ApiResult();
            try
            {
                
                OrgUser orguser = await _orgRepo.GetOrgUserByUserID(userId);
                if (orguser != null) {
                    ret = await GetOrgByIDs(new List<int> { orguser.NodeID });
                }
                else
                {
                    ret = await GetAllOrg();
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> GetOrgUserByUserID(int userId)
        {
            ApiResult ret = new ApiResult();
            try
            {

                OrgUser orguser = await _orgRepo.GetOrgUserByUserID(userId);
                if (orguser != null)
                {
                    ret = await GetOrgByIDs(new List<int> { orguser.NodeID });
                }
                else
                {
                    ret = await GetAllOrg();
                }
                List<OrgUser> users = await _orgRepo.ListAllOrgUser();
                ret.data = _mountUsers(ret.data as List<object>, users);
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> GetOrgUserByNodeID(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                ret = await GetOrgByIDs(new List<int> { id });
                List<OrgUser> users = await _orgRepo.ListAllOrgUser();
                ret.data = _mountUsers(ret.data as List<object>, users);
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        private List<object> _mountUsers(List<object> nodes, List<OrgUser> users)
        {
            for (int i = 0; i < nodes.Count; ++i)
            {
                object node = nodes[i];
                int id = Convert.ToInt32(node.GetType().GetProperty("id").GetValue(node));
                string label = node.GetType().GetProperty("label").GetValue(node).ToString();
                int node_type = Convert.ToInt32(node.GetType().GetProperty("node_type").GetValue(node));
                object type = node.GetType().GetProperty("type").GetValue(node);
                PropertyInfo pChildInfo = node.GetType().GetProperty("children");
                object children = null;
                if (pChildInfo != null)
                {
                    children = pChildInfo.GetValue(node);
                }
                List <OrgUser> curNodeUsers = users.Where(c => c.NodeID == id).ToList();
                List<object> nodeusers = curNodeUsers.Select(c => {return new {
                    id = id + "_" + c.ID,
                    label = c.UserName,
                    dataID = c.ID
                    };
                }).ToList<object>();
                if (children != null)
                {
                    List<object> nodeChildren = children as List<object>;
                    if (nodeChildren.Count == 0) // 无子节点也做为叶子节点
                    {
                        nodes[i] = new
                        {
                            id = id,
                            label = label,
                            node_type = node_type,
                            type = type,
                            children = nodeusers
                        };
                    }
                    else
                    {
                        _mountUsers(nodeChildren, users);
                        if (nodeusers.Count > 0)
                        {
                            var newNode = new
                            {
                                id = id + "_0",
                                label = "内部人员",
                                node_type = 0,
                                disabled = true,
                                children = nodeusers
                            };
                            nodeChildren.Insert(0, newNode);
                            nodes[i] = new
                            {
                                id = id,
                                label = label,
                                node_type = node_type,
                                type = type,
                                children = nodeChildren
                            };
                        }
                    }
                }
                else
                {
                    nodes[i] = new {
                        id = id,
                        label = label,
                        node_type = node_type,
                        type = type,
                        children = nodeusers
                    };
                }
            }
            return nodes;
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

        private object _parseOrgTree(OrgTree parentNode, List<OrgTree> nodes
            , List<OrgNodeType> nodeTypes)
        {
            OrgNodeType nodeType = nodeTypes.Where(c => c.ID == parentNode.NodeType)
                .FirstOrDefault();
            var node_p = new
            {
                id = parentNode.ID,
                label = parentNode.Name,
                node_type = parentNode.NodeType,
                type = nodeType,
                children = new List<object>()
            };
            List<OrgTree> nodes_children = nodes.Where(c => c.ParentID == parentNode.ID).ToList();
            if (nodes_children.Count > 0)
            {
                foreach (OrgTree child in nodes_children)
                {
                    if (!child.IsDel)
                    {
                        var node_c = _parseOrgTree(child, nodes, nodeTypes);
                        node_p.children.Add(node_c);
                    }
                }
            }
            if (node_p.children.Count == 0)
            {
                return new {
                    id = parentNode.ID,
                    label = parentNode.Name,
                    type = nodeType,
                    node_type = parentNode.NodeType,
                };
            }
            return node_p;
        }

        public async Task<ApiResult> AddOrgNode(OrgTree node)
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    List<OrgNodeType> nodeTypes = await _orgRepo.ListNodeType();
                    bool canAdd = true;
                    // 找到父节点，根据父节点类型判断是否可添加此节点
                    // 如果父节点可以有子节点，但属性为has_users_leafonly为true，且已关联人员则不能添加
                    if (node.ParentID != null)
                    {
                        OrgTree parent = await _orgRepo.GetNode((int)node.ParentID);
                        if (parent != null)
                        {
                            OrgNodeType nodeType = nodeTypes.Where(c => c.ID == parent.NodeType)
                                .FirstOrDefault();
                            if (!nodeType.HasChildren)
                            {
                                canAdd = false;
                            }
                            if (nodeType.HasUsersLeafOnly)
                            {
                                List<OrgUser> users = await _orgRepo.ListOrgNodeUsers(parent.ID);
                                if (users.Count > 0) {
                                    canAdd = false;
                                }
                            }
                        }
                    }
                    if (canAdd)
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

                            ret.code = Code.Success;
                            ret.data = data;
                        }
                        else
                        {
                            ret.code = Code.DataIsExist;
                        }
                    }
                    else
                    {
                        ret.code = Code.CheckDataRulesFail;
                    }
                    
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            
            return ret;
        }

        public async Task<ApiResult> UpdateOrgNode(OrgTree node)
        {
            ApiResult ret = new ApiResult();
            try {
                using (TransactionScope scope = new TransactionScope())
                {
                    List<OrgNodeType> nodeTypes = await _orgRepo.ListNodeType();
                    bool canUpdate = true;
                    // 如果此节点类型有子节点，则不可变为has_children为false的节点
                    OrgNodeType changeToNodeType = nodeTypes.Where(c => c.ID == node.NodeType)
                                .FirstOrDefault();
                    bool hasChildren = await _orgRepo.hasChildren(node.ID);

                    if (hasChildren && changeToNodeType != null && !changeToNodeType.HasChildren)
                    {
                        canUpdate = false;
                    }
                    // 如果此节点有用户，不可变为has_users 为false的节点
                    List<OrgUser> users = await _orgRepo.ListOrgNodeUsers(node.ID);
                    if (users.Count > 0 && changeToNodeType !=null && !changeToNodeType.HasUsers) {
                        canUpdate = false;
                    }

                    if (canUpdate)
                    {
                        // 检查是否存在同名节点
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
                            ret.code = Code.Success;
                            ret.data = data;
                        }
                        else
                        {
                            ret.code = Code.DataIsExist;
                        }
                    }
                    else
                    {
                        ret.code = Code.CheckDataRulesFail;
                    }
                    
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            
            return ret;
        }

        private async Task<bool> _saveNodeProperty (OrgTree node)
        {
            //获取此节点类型对应的属性
            List<OrgNodeTypeProperty> typeProps = await _orgRepo.ListNodeTypeProperty(node.NodeType);
            List<OrgNodeProperty> props = new List<OrgNodeProperty>();
            foreach (OrgNodeProperty prop in node.PropEx)
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

        public async Task<ApiResult> DeleteOrgNode(OrgTree node)
        {
            ApiResult ret = new ApiResult();
            try
            {
                OrgTree exist = await _orgRepo.GetNode(node.ID);
                if (exist == null)
                {
                    ret.code = Code.DataIsnotExist;
                    return ret;
                }
                await _orgRepo.DeleteOrgNode(node);
                await _orgRepo.UnbindOrgNodeUsers(node);
                ret.code = Code.Success;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }
            
            return ret;
        }

        public async Task<OrgNodeType> _getOrgNodeTypeByNodeID (int id)
        {
            List<OrgNodeType> nodeTypes = await _orgRepo.ListNodeType();
            OrgTree node = await _orgRepo.GetNode(id);
            OrgNodeType changeToNodeType = null;
            if (node != null)
            {
                changeToNodeType = nodeTypes.Where(c => c.ID == node.NodeType)
                                .FirstOrDefault();
            }
            return changeToNodeType;
        }

        public async Task<ApiResult> BindOrgNodeUsers(OrgUserView nodeView) {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    // //取消bind当前的用户
                    // OrgTree node = new OrgTree();
                    // node.ID = nodeView.ID;
                    // node.UpdatedBy = nodeView.CreatedBy;
                    // node.UpdatedTime = nodeView.CreatedTime;
                    // await _orgRepo.UnbindOrgNodeUsers(node);

                    // 判断此节点是否可绑定人员
                    bool canBind = true;
                    OrgNodeType nodeType = await _getOrgNodeTypeByNodeID(nodeView.ID);
                    if (nodeType != null && !nodeType.HasUsers)
                    {
                        canBind = false;
                    }
                    if (nodeType != null && nodeType.HasUsers && nodeType.HasUsersLeafOnly)
                    {
                        // 假如该节点目前为叶子节点后期有可能添加子节点，要提示不能添加
                        bool hasChildren = await _orgRepo.hasChildren(nodeView.ID);
                        if (hasChildren)
                        {
                            canBind = false;
                        }
                    }
                    if (canBind)
                    {
                        // 绑定前检查用户是否已被绑定
                        List<OrgUser> selectedUsers = await _orgRepo.ListAllOrgUser();
                        List<OrgUser> conflictUsers = selectedUsers
                            .Where(c => nodeView.UserIDs.Contains(c.UserID)).ToList();
                        if (conflictUsers.Count ==0)
                        {
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

                            ret.code = Code.Success;
                        }
                        else
                        {
                            ret.code =Code.BindUserConflict;
                        }
                    }
                    else
                    {
                        ret.code = Code.CheckDataRulesFail;
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }
        public async Task<ApiResult> DeleteOrgNodeUsers(OrgUserView nodeView)
        {
            ApiResult ret = new ApiResult();
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    await _orgRepo.DeleteOrgNodeUsers(nodeView);

                    ret.code = Code.Success;

                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> GetOrgNodeUsers(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                // 获取此节点已选择的用户
                List<OrgUser> users = await _orgRepo.ListOrgNodeUsers(id);

                // 获取此节点之外已选择的用户，此节点不可再选
                List<OrgUser> usersNotThisNode = await _orgRepo.ListUnOrgNodeUsers(id);
                
                ret.code = Code.Success;
                ret.data = new {
                    users = users,
                    disabledUsers = usersNotThisNode
                };
            }
            catch (Exception ex)
            {
                ret.code = Code.Success;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> GetCanSelectedUsers(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<User> users = await _orgRepo.ListUsersNotThisNode(id);

                ret.code = Code.Success;
                ret.data = users;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> GetNodeType() {
            ApiResult ret = new ApiResult();
            try
            {
                List<OrgNodeType> type = await _orgRepo.ListNodeType();

                ret.code = Code.Success;
                ret.data = type;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        public async Task<ApiResult> GetOrgNode(int id)
        {
            ApiResult ret = new ApiResult();
            try
            {
                OrgTree node = await _orgRepo.GetNodeView(id);

                ret.code = Code.Success;
                ret.data = node;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }

        // 获取所有已选用户
        public async Task<ApiResult> ListAllOrgUsers()
        {
            ApiResult ret = new ApiResult();
            try
            {
                List<OrgUser> users = await _orgRepo.ListAllOrgUser();

                ret.code = Code.Success;
                ret.data = users;
            }
            catch (Exception ex)
            {
                ret.code = Code.Failure;
                ret.msg = ex.Message;
            }

            return ret;
        }
    }
}
