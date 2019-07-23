using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Core.Common;
using MSS.API.Model.DTO;
using MSS.API.Common;
namespace MSS.API.Core.V1.Business
{
    public interface IOrgService
    {
        Task<ApiResult> GetAllOrg();
        Task<ApiResult> GetOrgByIDs(List<int> ids);
        Task<ApiResult> GetOrgByUserID(int userId);
        Task<ApiResult> GetOrgUserByUserID(int userId);
        Task<ApiResult> GetOrgUserByNodeID(int id);
        Task<ApiResult> AddOrgNode(OrgTree node);
        Task<ApiResult> UpdateOrgNode(OrgTree node);
        Task<ApiResult> DeleteOrgNode(OrgTree node);
        Task<ApiResult> GetOrgNodeUsers(int id);
        Task<ApiResult> GetCanSelectedUsers(int id);
        Task<ApiResult> BindOrgNodeUsers(OrgUserView nodeView);
        Task<ApiResult> GetNodeType();
        Task<ApiResult> GetOrgNode(int id);

        // 获取所有已选用户
        Task<ApiResult> ListAllOrgUsers();

        // 删除节点关联的用户
        Task<ApiResult> DeleteOrgNodeUsers(OrgUserView nodeView);

        // 根据用户ID获取所在顶级组织节点
        Task<ApiResult> GetTopNodeByUserID(int id);

        // 获取所有顶级节点下所有用户，包括子级节点的用户
        Task<ApiResult> ListTopNodeWithUsers();
    }
}
