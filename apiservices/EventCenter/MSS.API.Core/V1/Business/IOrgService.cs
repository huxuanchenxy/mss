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
        // 获取所有顶级节点下所有用户，包括子级节点的用户
        Task<ApiResult> ListTopNodeWithUsers();
    }
}
