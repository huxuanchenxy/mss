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
    public interface IUploadFileService
    {
        Task<ApiResult> Save(int type,int systemResource, List<IFormFile> file);
        Task<ApiResult> Delete(int id);
        Task<ApiResult> GetByID(int id);
        Task<ApiResult> ListByIDs(string ids);
        Task<ApiResult> ListAll();
    }
}
