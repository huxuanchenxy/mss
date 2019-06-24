﻿using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Core.Common;
using MSS.API.Model.DTO;
namespace MSS.API.Core.V1.Business
{
    public interface IOrgService
    {
        Task<DataResult> GetAllOrg();
        Task<DataResult> GetOrgByIDs(List<int> ids);
        Task<DataResult> AddOrgNode(OrgTree node);
        Task<DataResult> UpdateOrgNode(OrgTree node);
        Task<DataResult> DeleteOrgNode(OrgTree node);
        Task<DataResult> GetOrgNodeUsers(int id);
        Task<DataResult> GetCanSelectedUsers(int id);
        Task<DataResult> BindOrgNodeUsers(OrgUserView nodeView);
        Task<DataResult> GetNodeType();
        Task<DataResult> GetOrgNode(int id);
    }
}
