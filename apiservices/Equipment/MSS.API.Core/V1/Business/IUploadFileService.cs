using MSS.API.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MSS.API.Common;
using MSS.API.Model.DTO;
using Microsoft.AspNetCore.Http;
using static MSS.API.Common.MyDictionary;

namespace MSS.API.Core.V1.Business
{
    public interface IUploadFileService
    {
        Task<ApiResult> Save(int type,int systemResource, List<IFormFile> file);
        Task<ApiResult> Save(List<UploadFileRelation> ufrs);
        Task<ApiResult> Delete(int id);
        Task<ApiResult> GetByID(int id);
        Task<ApiResult> ListByIDs(string ids);
        Task<ApiResult> ListByEntity(int[] entitys, SystemResource sr, UploadShowType ust);
        Task<ApiResult> ListByEntity(int[] entitys, SystemResource sr);
        Task<ApiResult> ListByEqp(int id);
        Task<ApiResult> ListAll();
        Task<ApiResult> CascaderByIDs(string ids);
    }
}
