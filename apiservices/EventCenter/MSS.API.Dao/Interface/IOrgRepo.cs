using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MSS.API.Model.DTO;

namespace MSS.API.Dao.Interface
{
    public interface IOrgRepo<T> where T:BaseEntity
    {
        Task<List<OrgTree>> ListAllOrgNode();

        // 取出所有已关联组织的用户
        Task<List<OrgUser>> ListAllOrgUser();

        Task<List<OrgUser>> ListOrgNodeUsers(int id);
    }
}
