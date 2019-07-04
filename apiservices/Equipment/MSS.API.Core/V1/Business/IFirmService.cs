using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Common;
using MSS.API.Model.DTO;
using Microsoft.AspNetCore.Http;

namespace MSS.API.Core.V1.Business
{
    public interface IFirmService
    {
        Task<ApiResult> Save(Firm firm);
        Task<ApiResult> Update(Firm firm);
        Task<ApiResult> Delete(string ids, int userID);
        Task<ApiResult> GetPageByParm(FirmQueryParm parm);
        Task<ApiResult> GetByID(int id);
        Task<ApiResult> GetAll();
    }
}
