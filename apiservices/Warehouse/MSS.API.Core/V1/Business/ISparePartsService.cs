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
    public interface ISparePartsService
    {
        Task<ApiResult> Save(SpareParts spareParts);
        Task<ApiResult> Update(SpareParts spareParts);
        Task<ApiResult> Delete(string ids);
        Task<ApiResult> GetPageByParm(SparePartsQueryParm parm);
        Task<ApiResult> GetByID(int id);
        Task<ApiResult> GetAll();
    }
}
