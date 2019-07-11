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
        Task<DataResult> GetAllOrg();
        Task<DataResult> GetOrgByIDs(List<int> ids);
        Task<DataResult> GetOrgByUserID(int userId);
        Task<DataResult> GetOrgUserByUserID(int userId);
        Task<DataResult> GetOrgUserByNodeID(int id);
        Task<ApiResult> AddOrgNode(OrgTree node);
        Task<ApiResult> UpdateOrgNode(OrgTree node);
        Task<DataResult> DeleteOrgNode(OrgTree node);
        Task<DataResult> GetOrgNodeUsers(int id);
        Task<DataResult> GetCanSelectedUsers(int id);
        Task<ApiResult> BindOrgNodeUsers(OrgUserView nodeView);
        Task<DataResult> GetNodeType();
        Task<DataResult> GetOrgNode(int id);

        // 获取所有已选用户
        Task<ApiResult> ListAllOrgUsers();

        // 删除节点关联的用户
        Task<ApiResult> DeleteOrgNodeUsers(OrgUserView nodeView);
    }
}
